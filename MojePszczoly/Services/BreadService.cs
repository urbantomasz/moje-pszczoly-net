using MojePszczoly.Data;
using MojePszczoly.Data.Models;
using MojePszczoly.Interfaces;

namespace MojePszczoly.Services
{
    public class BreadService : IBreadService
    {
        private readonly AppDbContext _context;
        public BreadService(AppDbContext context) {
            _context =context;
        }
        public List<Bread> GetBreads()
        {
            return _context.Breads.ToList();
        }
    }
}
