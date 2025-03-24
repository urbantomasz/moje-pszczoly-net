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

            // Konwersja z powrotem do UTC przed wysłaniem
            return new List<DateTime> { nextTuesday, nextWednesday, nextThursday }
                .Select(d => TimeZoneInfo.ConvertTimeToUtc(d, polandTimeZone))
                .OrderBy(d => d.Date)
                .ToList();
        }

        private DateTime GetNextWeekday(DateTime startDate, DayOfWeek targetDay)
        {
            int diff = ((int)targetDay - (int)startDate.DayOfWeek + 7) % 7;
            return startDate.AddDays(diff == 0 ? 7 : diff).Date;  // Zachowaj oryginalną strefę czasową
        }
    }

}
