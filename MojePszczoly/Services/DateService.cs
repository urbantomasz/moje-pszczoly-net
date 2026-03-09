using MojePszczoly.Interfaces;

namespace MojePszczoly.Services
{
    public class DateService : IDateService
    {
        private readonly IDateOverrideService _overrideService;
        private readonly IDateTimeProvider _dateTimeProvider;
        public DateService(IDateOverrideService overrideService, IDateTimeProvider dateTimeProvider)
        {
            _overrideService = overrideService;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<List<DateOnly>> GetUpcomingDates()
        {
            var today = _dateTimeProvider.GetPolandNow();

            var baseDates = new List<DateOnly>
            {
                GetNextWeekday(today, DayOfWeek.Tuesday),
                GetNextWeekday(today, DayOfWeek.Wednesday),
                GetNextWeekday(today, DayOfWeek.Thursday)
            }
            .ToList();

            var overrides = await _overrideService.GetOverrides();

            var excluded = overrides
                .Where(o => !o.IsAdded)
                .Select(o => o.Date)
                .ToList();

            var added = overrides
                .Where(o => o.IsAdded)
                .Select(o => o.Date)
                .ToList();

            var result = baseDates
                .Where(d => !excluded.Contains(d))
                .Concat(added)
                .Distinct()
                .OrderBy(d => d)
                .ToList();

            return result;
        }

        public List<DateOnly> GetCurrentWeekDates()
        {
            var today = _dateTimeProvider.GetPolandNow();

            int diffToMonday = ((int)today.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7;
            var monday = today.AddDays(-diffToMonday);

            var tuesday = monday.AddDays(1);
            var wednesday = monday.AddDays(2);
            var thursday = monday.AddDays(3);

            return new List<DateOnly> { tuesday, wednesday, thursday }.ToList();
        }

        private DateOnly GetNextWeekday(DateOnly startDate, DayOfWeek targetDay)
        {
            int diff = ((int)targetDay - (int)startDate.DayOfWeek + 7) % 7;
            return startDate.AddDays(diff == 0 ? 7 : diff); 
        }

        public DateOnly GetCurrentWeekMonday()
        {
            var today = _dateTimeProvider.GetPolandNow();

            int diffToMonday = ((int)today.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7;
            var monday = today.AddDays(-diffToMonday);

            return monday;
        }
    }
}
