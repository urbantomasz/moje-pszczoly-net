using MojePszczoly.Application.Interfaces.Repositories;
using MojePszczoly.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MojePszczoly.Infrastructure.Repositories
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