using System;
using System.Collections.Generic;
using System.Linq;

namespace MojePszczoly.Services
{
    public class DateService
    {
        public List<DateTime> GetUpcomingDates()
        {
            TimeZoneInfo polandTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
            var today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, polandTimeZone);

            var nextTuesday = GetNextWeekday(today, DayOfWeek.Tuesday);
            var nextWednesday = GetNextWeekday(today, DayOfWeek.Wednesday);
            var nextThursday = GetNextWeekday(today, DayOfWeek.Thursday);

            return new List<DateTime> { nextTuesday, nextWednesday, nextThursday }
                .Select(d => new DateTime(d.Year, d.Month, d.Day, 0, 0, 0, DateTimeKind.Utc))
                .OrderBy(d => d)
                .ToList();
        }


        private DateTime GetNextWeekday(DateTime startDate, DayOfWeek targetDay)
        {
            int diff = ((int)targetDay - (int)startDate.DayOfWeek + 7) % 7;
            return startDate.AddDays(diff == 0 ? 7 : diff).Date; 
        }
    }

}
