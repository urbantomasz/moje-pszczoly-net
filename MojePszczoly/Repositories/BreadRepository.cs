using Microsoft.EntityFrameworkCore;
using MojePszczoly.Infrastructure;
using MojePszczoly.Infrastructure.Entities;
using MojePszczoly.Interfaces.Repositories;

namespace MojePszczoly.Repositories
{
    public class BreadRepository : IBreadRepository
    {
        private readonly AppDbContext _context;

        public BreadRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Bread>> GetAllAsync()
        {
            return await GetOrderedAsync();
        }

        public async Task<List<Bread>> GetOrderedAsync()
        {
            return await _context.Breads
                .AsNoTracking()
                .OrderBy(bread => bread.SortOrder)
                .ToListAsync();
        }
    }
}