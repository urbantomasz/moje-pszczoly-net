using MojePszczoly.Data;
using MojePszczoly.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MojePszczoly.Services
{
    public class DateService : IDateService
    {
        private readonly AppDbContext _context;
        public DateService(AppDbContext dbContext)
        {
            _context = dbContext;
        }

        public List<DateTime> GetUpcomingDates()
        {
            TimeZoneInfo polandTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
            var today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, polandTimeZone);

            var nextTuesday = GetNextWeekday(today, DayOfWeek.Tuesday);
            var nextWednesday = GetNextWeekday(today, DayOfWeek.Wednesday);
            var nextThursday = GetNextWeekday(today, DayOfWeek.Thursday);

            return new List<DateTime> {  nextThursday }
                .Select(d => new DateTime(d.Year, d.Month, d.Day, 0, 0, 0, DateTimeKind.Utc))
                .OrderBy(d => d)
                .ToList();
        }


        //public List<DateTime> GetUpcomingDates()
        //{
        //    int currentYear = DateTime.UtcNow.Year;

        //    var april29 = new DateTime(currentYear, 6, 17, 0, 0, 0, DateTimeKind.Utc);
        //    var may2 = new DateTime(currentYear, 6, 18, 0, 0, 0, DateTimeKind.Utc);

        //    return new List<DateTime> { april29, may2 };
        //}

        public List<DateTime> GetCurrentWeekDates()
        {
            TimeZoneInfo polandTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
            var today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, polandTimeZone);

            int diffToMonday = ((int)today.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7;
            var monday = today.Date.AddDays(-diffToMonday);

            var tuesday = monday.AddDays(1);
            var wednesday = monday.AddDays(2);
            var thursday = monday.AddDays(3);

            return new List<DateTime> { tuesday, wednesday, thursday }
                .Select(d => new DateTime(d.Year, d.Month, d.Day, 0, 0, 0, DateTimeKind.Utc))
                .ToList();
        }


        public List<DateTime> GetPastDates()
        {
            return _context.Orders
                .Select(o => o.OrderDate)
                .Where(d => d < DateTime.UtcNow)
                .Distinct()
                .ToList();
        }

        private DateTime GetNextWeekday(DateTime startDate, DayOfWeek targetDay)
        {
            int diff = ((int)targetDay - (int)startDate.DayOfWeek + 7) % 7;
            return startDate.AddDays(diff == 0 ? 7 : diff).Date; 
        }


        public DateTime GetCurrentWeekMonday()
        {
            TimeZoneInfo polandTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
            var today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, polandTimeZone);

            int diffToMonday = ((int)today.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7;
            var monday = today.Date.AddDays(-diffToMonday);

            return new DateTime(monday.Year, monday.Month, monday.Day, 0, 0, 0, DateTimeKind.Utc);
        }
    }

}
