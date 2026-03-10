using MojePszczoly.Contracts.Requests;
using MojePszczoly.Contracts.Responses;

namespace MojePszczoly.Application.Interfaces
{
    public interface IOrderService
    {
        Task CreateOrder(CreateOrderRequest orderDto);
        Task<List<OrderResponse>> GetOrders();
        Task<List<OrderResponse>> GetOrders(DateOnly date);
        Task<List<OrderResponse>> GetPastOrders();
        Task<bool> DeleteOrder(int id);
        Task<bool> UpdateOrder(int id, UpdateOrderRequest updatedOrder);
    }
}
