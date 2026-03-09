using Microsoft.EntityFrameworkCore;
using MojePszczoly.Contracts.Responses;
using MojePszczoly.Infrastructure;
using MojePszczoly.Interfaces;

namespace MojePszczoly.Services
{
    public class BreadService : IBreadService
    {
        private readonly AppDbContext _context;
        public BreadService(AppDbContext context) {
            _context =context;
        }
        public async Task<List<BreadResponse>> GetBreads()
        {
            return await _context.Breads.AsNoTracking()
                .Select(b => new BreadResponse
                {
                    BreadId = b.BreadId,
                    Name = b.Name,
                    ShortName = b.ShortName,
                    SortOrder = b.SortOrder
                })
                .ToListAsync();
        }
    }
}
