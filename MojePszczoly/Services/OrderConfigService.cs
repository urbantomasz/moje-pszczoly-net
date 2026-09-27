using Microsoft.EntityFrameworkCore;
using MojePszczoly.Data;
using MojePszczoly.Data.Models;
using MojePszczoly.Interfaces;
using MojePszczoly.Models;

namespace MojePszczoly.Services
{
    public class OrderConfigService : IOrderConfigService
    {
        private const int MaxNoticeTitleLength = 120;
        private const int MaxNoticeMessageLength = 500;
        private const int MaxNoteLength = 200;

        private readonly AppDbContext _context;
        private readonly IDateService _dateService;

        public OrderConfigService(AppDbContext context, IDateService dateService)
        {
            _context = context;
            _dateService = dateService;
        }

        public async Task<PublicOrderConfigDto> GetPublicAsync(CancellationToken ct = default)
        {
            var settings = await GetSettingsAsNoTrackingAsync(ct);

            var notice = settings.NoticeEnabled && !string.IsNullOrWhiteSpace(settings.NoticeMessage)
                ? new NoticeDto(settings.NoticeTitle, settings.NoticeMessage)
                : null;

            return new PublicOrderConfigDto(notice, _dateService.GetUpcomingOrderDates());
        }

        public async Task<AdminOrderConfigDto> GetForAdminAsync(CancellationToken ct = default)
        {
            var settings = await GetSettingsAsNoTrackingAsync(ct);

            var customDates = (await _context.CustomOrderDates
                    .AsNoTracking()
                    .OrderBy(d => d.DateUtc)
                    .Select(d => new { d.DateUtc, d.Note })
                    .ToListAsync(ct))
                .Select(d => new OrderDateDto(DateTime.SpecifyKind(d.DateUtc, DateTimeKind.Utc), d.Note))
                .ToList();

            return new AdminOrderConfigDto(
                settings.UseCustomDates,
                settings.NoticeEnabled,
                settings.NoticeTitle,
                settings.NoticeMessage,
                customDates,
                _dateService.GetGeneratedDates(),
                DateTime.SpecifyKind(settings.UpdatedAt, DateTimeKind.Utc)
            );
        }

        public async Task UpdateAsync(UpdateOrderConfigRequest request, CancellationToken ct = default)
        {
            var incoming = Normalize(request.CustomDates);

            if (request.UseCustomDates && incoming.Count == 0)
                throw new ArgumentException("Wlaczone wlasne daty wymagaja co najmniej jednej daty.");

            if (request.NoticeEnabled && string.IsNullOrWhiteSpace(request.NoticeMessage))
                throw new ArgumentException("Wlaczony komunikat wymaga tresci.");

            if (request.NoticeTitle?.Length > MaxNoticeTitleLength)
                throw new ArgumentException($"Tytul komunikatu moze miec najwyzej {MaxNoticeTitleLength} znakow.");

            if (request.NoticeMessage?.Length > MaxNoticeMessageLength)
                throw new ArgumentException($"Tresc komunikatu moze miec najwyzej {MaxNoticeMessageLength} znakow.");

            if (incoming.Any(d => d.Note?.Length > MaxNoteLength))
                throw new ArgumentException($"Notatka przy dacie moze miec najwyzej {MaxNoteLength} znakow.");

            await using var transaction = await _context.Database.BeginTransactionAsync(ct);

            var settings = await _context.OrderFormSettings.FirstOrDefaultAsync(s => s.Id == 1, ct);
            if (settings is null)
            {
                settings = new OrderFormSettings { Id = 1 };
                _context.OrderFormSettings.Add(settings);
            }

            settings.UseCustomDates = request.UseCustomDates;
            settings.NoticeEnabled = request.NoticeEnabled;
            settings.NoticeTitle = Trimmed(request.NoticeTitle);
            settings.NoticeMessage = Trimmed(request.NoticeMessage);
            settings.UpdatedAt = DateTime.UtcNow;

            await ReconcileDatesAsync(incoming, ct);

            await _context.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);
        }

        /// <summary>
        /// Dopasowuje istniejace wiersze po dacie zamiast kasowac wszystko i wstawiac od nowa,
        /// dzieki czemu niezmienione daty zachowuja swoje Id.
        /// </summary>
        private async Task ReconcileDatesAsync(List<OrderDateDto> incoming, CancellationToken ct)
        {
            var existing = await _context.CustomOrderDates.ToListAsync(ct);
            var incomingByDate = incoming.ToDictionary(d => d.Date, d => d.Note);

            foreach (var row in existing)
            {
                if (incomingByDate.TryGetValue(row.DateUtc, out var note))
                {
                    row.Note = note;
                    incomingByDate.Remove(row.DateUtc);
                }
                else
                {
                    _context.CustomOrderDates.Remove(row);
                }
            }

            foreach (var (date, note) in incomingByDate)
            {
                _context.CustomOrderDates.Add(new CustomOrderDate { DateUtc = date, Note = note });
            }
        }

        private async Task<OrderFormSettings> GetSettingsAsNoTrackingAsync(CancellationToken ct)
        {
            return await _context.OrderFormSettings.AsNoTracking().FirstOrDefaultAsync(s => s.Id == 1, ct)
                   ?? new OrderFormSettings { Id = 1 };
        }

        /// <summary>Sprowadza daty do polnocy UTC, usuwa duplikaty i porzadkuje rosnaco.</summary>
        private static List<OrderDateDto> Normalize(IEnumerable<OrderDateDto>? dates)
        {
            return (dates ?? Enumerable.Empty<OrderDateDto>())
                .Select(d => new OrderDateDto(
                    new DateTime(d.Date.Year, d.Date.Month, d.Date.Day, 0, 0, 0, DateTimeKind.Utc),
                    Trimmed(d.Note)))
                .GroupBy(d => d.Date)
                .Select(g => g.First())
                .OrderBy(d => d.Date)
                .ToList();
        }

        private static string? Trimmed(string? value)
            => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
