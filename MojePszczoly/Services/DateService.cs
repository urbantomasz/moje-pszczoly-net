using System.Globalization;

namespace MojePszczoly.Services
{
    public class DateService
    {
        public List<DateTime> GetUpcomingDates()
        {
            var today = DateTime.Today.Date;
            var nextTuesday = GetNextWeekday(today, DayOfWeek.Tuesday);
            var nextWednesday = GetNextWeekday(today, DayOfWeek.Wednesday);
            var nextThursday = GetNextWeekday(today, DayOfWeek.Thursday);

            return new List<DateTime>
            {
                nextTuesday,
                nextWednesday,
                nextThursday
            }
            .OrderBy(d => d.Date)
            .ToList();
        }

        private DateTime GetNextWeekday(DateTime startDate, DayOfWeek targetDay)
        {
            int diff = ((int)targetDay - (int)startDate.DayOfWeek + 7) % 7;
            return startDate.AddDays(diff == 0 ? 7 : diff); // Ensure next occurrence
        }
    }
}
