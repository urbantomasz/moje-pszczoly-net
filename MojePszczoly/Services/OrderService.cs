using Microsoft.EntityFrameworkCore;
using MojePszczoly.Contracts.Dtos;
using MojePszczoly.Contracts.Requests;
using MojePszczoly.Contracts.Responses;
using MojePszczoly.Infrastructure;
using MojePszczoly.Infrastructure.Entities;
using MojePszczoly.Interfaces;

namespace MojePszczoly.Services
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _context;
        private readonly IDateService _dateService;

        public OrderService(AppDbContext context, IDateService dateService)
        {
            _context = context;
            _dateService = dateService;
        }

        public async Task CreateOrder(CreateOrderRequest orderDto)
        {
            var order = new Order
            {
                CustomerName = orderDto.CustomerName,
                Phone = orderDto.Phone,
                OrderDate = orderDto.OrderDate,
                Note = orderDto.Note,
                Items = orderDto.Items.Select(itemDto => new OrderItem
                {
                    BreadId = itemDto.BreadId,
                    Quantity = itemDto.Quantity
                }).ToList()
            };

            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
        }

        public async Task<List<OrderResponse>> GetOrders(DateOnly date)
        {
                var orderEntities = await _context.Orders
                    .Include(o => o.Items)
                    .ThenInclude(i => i.Bread)
                    .AsNoTracking()
                    .Where(o => o.OrderDate == date)
                    .OrderByDescending(o => o.CreatedAt)
                    .ToListAsync();

                orderEntities.ForEach(o =>
                {
                    o.CreatedAt = DateTime.SpecifyKind(o.CreatedAt, DateTimeKind.Utc);
                });

                var orders = orderEntities.Select(o => new OrderResponse
                {
                    OrderId = o.OrderId,
                    CustomerName = o.CustomerName,
                    Note = o.Note,
                    Phone = o.Phone,
                    CreatedAt = o.CreatedAt,
                    OrderDate = o.OrderDate,
                    Items = o.Items.Select(i => new OrderItemDto
                    {
                        BreadId = i.BreadId,
                        Quantity = i.Quantity
                    }).ToList()
                }).ToList();
            
            return orders;
        }

        public async Task<List<OrderResponse>> GetOrders()
        {
            return await GetOrdersInternal(isCurrent: true);
        }

        public async Task<List<OrderResponse>> GetPastOrders()
        {
            return await GetOrdersInternal(isCurrent: false);
        }

        private async Task<List<OrderResponse>> GetOrdersInternal(bool isCurrent)
        {
            var currentWeekMonday = _dateService.GetCurrentWeekMonday();

            var query =  _context.Orders
            .Include(o => o.Items)
            .ThenInclude(i => i.Bread)
            .AsNoTracking();

            query = isCurrent
                ? query.Where(o => o.OrderDate >= currentWeekMonday)
                : query.Where(o => o.OrderDate < currentWeekMonday);

            var orderEntities = await query
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();

            orderEntities.ForEach(o =>
            {
                o.CreatedAt = DateTime.SpecifyKind(o.CreatedAt, DateTimeKind.Utc);
            });

            var orders = orderEntities.Select(o => new OrderResponse
            {
                OrderId = o.OrderId,
                CustomerName = o.CustomerName,
                Note = o.Note,
                Phone = o.Phone,
                CreatedAt = o.CreatedAt,
                OrderDate = o.OrderDate,
                Items = o.Items.Select(i => new OrderItemDto
                {
                    BreadId = i.BreadId,
                    Quantity = i.Quantity
                }).ToList()
            }).ToList();

            return orders;
        }

        public async Task<bool> DeleteOrder(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
                return false;

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
         
            return true;
        }

        public async Task<bool> UpdateOrder(int id, UpdateOrderRequest updatedOrder)
        {
            var existingOrder = await _context.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (existingOrder == null)
                return false;

            existingOrder.CustomerName = updatedOrder.CustomerName;
            existingOrder.Phone = updatedOrder.Phone;
            existingOrder.OrderDate = updatedOrder.OrderDate;
            existingOrder.Note = updatedOrder.Note;

            _context.OrderItems.RemoveRange(existingOrder.Items);

            existingOrder.Items = updatedOrder.Items
                .Select(item => new OrderItem
                {
                    BreadId = item.BreadId,
                    Quantity = item.Quantity,
                    OrderId = id
                }).ToList();

            await _context.SaveChangesAsync();
          
            return true;
        }
    }   
}
