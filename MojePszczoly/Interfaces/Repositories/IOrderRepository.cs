using MojePszczoly.Infrastructure.Entities;

namespace MojePszczoly.Interfaces.Repositories
{
    public interface IOrderRepository
    {
        Task AddAsync(Order order);
        Task<List<Order>> GetByDateAsync(DateOnly date);
        Task<List<Order>> GetCurrentAsync(DateOnly currentWeekMonday);
        Task<List<Order>> GetPastAsync(DateOnly currentWeekMonday);
        Task<Order?> GetByIdWithItemsAsync(int id);
        Task<bool> DeleteAsync(Order order);
        Task ReplaceItemsAsync(Order order, List<OrderItem> items);
    }
}