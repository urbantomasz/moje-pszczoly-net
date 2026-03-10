using MojePszczoly.Application.Interfaces.Repositories;
using MojePszczoly.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using MojePszczoly.Infrastructure;

namespace MojePszczoly.Infrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _context;

        public OrderRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
        }

        public async Task<List<Order>> GetByDateAsync(DateOnly date)
        {
            return await BuildOrderQuery()
                .Where(order => order.OrderDate == date)
                .OrderByDescending(order => order.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Order>> GetCurrentAsync(DateOnly currentWeekMonday)
        {
            return await BuildOrderQuery()
                .Where(order => order.OrderDate >= currentWeekMonday)
                .OrderByDescending(order => order.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Order>> GetPastAsync(DateOnly currentWeekMonday)
        {
            return await BuildOrderQuery()
                .Where(order => order.OrderDate < currentWeekMonday)
                .OrderByDescending(order => order.CreatedAt)
                .ToListAsync();
        }

        public async Task<Order?> GetByIdWithItemsAsync(int id)
        {
            return await _context.Orders
                .Include(order => order.Items)
                .FirstOrDefaultAsync(order => order.OrderId == id);
        }

        public Task<bool> DeleteAsync(Order order)
        {
            _context.Orders.Remove(order);
            return Task.FromResult(true);
        }

        public Task ReplaceItemsAsync(Order order, List<OrderItem> items)
        {
            _context.OrderItems.RemoveRange(order.Items);
            order.Items = items;
            return Task.CompletedTask;
        }

        private IQueryable<Order> BuildOrderQuery()
        {
            return _context.Orders
                .Include(order => order.Items)
                .ThenInclude(item => item.Bread)
                .AsNoTracking();
        }
    }
}