using MojePszczoly.Application.Interfaces.Repositories;
using MojePszczoly.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using MojePszczoly.Infrastructure;

namespace MojePszczoly.Infrastructure.Repositories
{
    public class DateOverrideRepository : IDateOverrideRepository
    {
        private readonly AppDbContext _context;

        public DateOverrideRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<DateOverride?> GetByDateAsync(DateOnly date)
        {
            return await _context.DateOverrides.FirstOrDefaultAsync(overrideEntry => overrideEntry.Date == date);
        }

        public async Task AddAsync(DateOverride overrideEntry)
        {
            await _context.DateOverrides.AddAsync(overrideEntry);
        }

        public Task<bool> DeleteAsync(DateOverride overrideEntry)
        {
            _context.DateOverrides.Remove(overrideEntry);
            return Task.FromResult(true);
        }

        public async Task<List<DateOverride>> GetFromDateAsync(DateOnly date)
        {
            return await _context.DateOverrides
                .AsNoTracking()
                .Where(overrideEntry => overrideEntry.Date >= date)
                .OrderBy(overrideEntry => overrideEntry.Date)
                .ToListAsync();
        }

        public async Task<List<DateOverride>> GetBetweenDatesAsync(DateOnly startDate, DateOnly endDate)
        {
            return await _context.DateOverrides
                .AsNoTracking()
                .Where(overrideEntry => overrideEntry.Date >= startDate && overrideEntry.Date <= endDate)
                .OrderBy(overrideEntry => overrideEntry.Date)
                .ToListAsync();
        }
    }
}