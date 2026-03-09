using Microsoft.EntityFrameworkCore;
using MojePszczoly.Infrastructure;
using MojePszczoly.Infrastructure.Entities;
using MojePszczoly.Interfaces;

namespace MojePszczoly.Services
{
    public class DateOverrideService : IDateOverrideService
    {
        private readonly AppDbContext _context;
        private readonly IDateTimeProvider _dateTimeProvider;

        public DateOverrideService(AppDbContext context, IDateTimeProvider dateTimeProvider)
        {
            _context = context;
            _dateTimeProvider = dateTimeProvider;
        }


        public async Task AddExtraDay(DateOnly date)
        {
            var existing = await _context.DateOverrides.FirstOrDefaultAsync(x => x.Date == date);
            if (existing != null)
            {
                if (existing.IsAdded)
                    return;
                existing.IsAdded = true;
            }
            else
            {
                _context.DateOverrides.Add(new DateOverride { Date = date, IsAdded = true });
            }
            await _context.SaveChangesAsync();
        }

        public async Task ExcludeDay(DateOnly date)
        {
            var existing = await _context.DateOverrides.FirstOrDefaultAsync(x => x.Date == date);
            if (existing != null)
            {
                if (!existing.IsAdded)
                    return;
                existing.IsAdded = false;
            }
            else
            {
                _context.DateOverrides.Add(new DateOverride { Date = date, IsAdded = false });
            }
            await _context.SaveChangesAsync();
        }

        public async Task<bool> RevertOverride(DateOnly date)
        {
            var existing = await _context.DateOverrides.FirstOrDefaultAsync(x => x.Date == date);
            if (existing == null) return false;
            _context.DateOverrides.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<DateOverride>> GetOverridesForNextTwoWeeks()
        {
            var today = _dateTimeProvider.GetPolandNow();
            var endDate = GetEndOfNextWeek(today);

            return await _context.DateOverrides
                .AsNoTracking()
                .Where(x => x.Date >= today && x.Date <= endDate)
                .OrderBy(d => d.Date)
                .ToListAsync();
        }

          public async Task<List<DateOverride>> GetOverrides()
        {
            var today = _dateTimeProvider.GetPolandNow();
            var list = await _context.DateOverrides.AsNoTracking().Where(x => x.Date >= today).OrderBy(d => d.Date).ToListAsync();
            return list;
        }

        private DateOnly GetEndOfNextWeek(DateOnly today)
        {
            int daysUntilSunday = ((int)DayOfWeek.Sunday - (int)today.DayOfWeek + 7) % 7;
            var thisSunday = today.AddDays(daysUntilSunday);

            return thisSunday.AddDays(7); // niedziela następnego tygodnia
        }
    }
}
