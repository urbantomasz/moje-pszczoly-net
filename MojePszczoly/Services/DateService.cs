using MojePszczoly.Data;
using MojePszczoly.Interfaces;
using MojePszczoly.Models;
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

        /// <summary>
        /// Daty widoczne w formularzu zamowienia. Jesli w panelu wlaczono wlasne daty
        /// i sa jakies nadchodzace - one wygrywaja. W przeciwnym razie wracamy do
        /// wyliczanych wtorku/srody/czwartku.
        /// </summary>
        public List<OrderDateDto> GetUpcomingOrderDates()
        {
            var settings = _context.OrderFormSettings.FirstOrDefault(s => s.Id == 1);

            if (settings is { UseCustomDates: true })
            {
                var today = GetPolandTodayAsUtc();

                var customDates = _context.CustomOrderDates
                    .Where(d => d.DateUtc >= today)
                    .OrderBy(d => d.DateUtc)
                    .Select(d => new { d.DateUtc, d.Note })
                    .AsEnumerable()
                    .Select(d => new OrderDateDto(DateTime.SpecifyKind(d.DateUtc, DateTimeKind.Utc), d.Note))
                    .ToList();

                if (customDates.Count > 0)
                    return customDates;
            }

            return GetGeneratedDates()
                .Select(d => new OrderDateDto(d, null))
                .ToList();
        }

        public List<DateTime> GetUpcomingDates()
        {
            return GetUpcomingOrderDates().Select(d => d.Date).ToList();
        }

        /// <summary>Normalny rytm tygodnia, bez ogladania sie na konfiguracje.</summary>
        public List<DateTime> GetGeneratedDates()
        {
            var today = GetPolandNow();

            var nextTuesday = GetNextWeekday(today, DayOfWeek.Tuesday);
            var nextWednesday = GetNextWeekday(today, DayOfWeek.Wednesday);
            var nextThursday = GetNextWeekday(today, DayOfWeek.Thursday);

            return new List<DateTime> { nextTuesday, nextWednesday, nextThursday }
                .Select(d => new DateTime(d.Year, d.Month, d.Day, 0, 0, 0, DateTimeKind.Utc))
                .OrderBy(d => d)
                .ToList();
        }

        public List<DateTime> GetCurrentWeekDates()
        {
            var today = GetPolandNow();

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
            var today = GetPolandNow();

            int diffToMonday = ((int)today.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7;
            var monday = today.Date.AddDays(-diffToMonday);

            return new DateTime(monday.Year, monday.Month, monday.Day, 0, 0, 0, DateTimeKind.Utc);
        }

        private static DateTime GetPolandNow()
        {
            TimeZoneInfo polandTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, polandTimeZone);
        }

        /// <summary>Dzisiejsza data w Polsce, sprowadzona do polnocy UTC - tak jak trzymamy daty dostaw.</summary>
        private static DateTime GetPolandTodayAsUtc()
        {
            var today = GetPolandNow();
            return new DateTime(today.Year, today.Month, today.Day, 0, 0, 0, DateTimeKind.Utc);
        }
    }

}
