using MojePszczoly.Models;

namespace MojePszczoly.Interfaces
{
    public interface IOrderService
    {
        void CreateOrder(CreateOrderDto orderDto);
        Task<List<OrderDto>> GetOrders();
        Task<List<OrderDto>> GetOrders(DateTime dateTime);
        Task<List<OrderDto>> GetPastOrders();
        Task<bool> DeleteOrder(int id);
        Task<bool> UpdateOrder(int id, OrderUpdateDto updatedOrder);
        Task<MemoryStream> GetOrdersReportExcel(DateTime date);
    }
}
