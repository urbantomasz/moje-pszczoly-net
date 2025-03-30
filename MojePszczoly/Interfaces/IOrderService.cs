using MojePszczoly.Models;

namespace MojePszczoly.Interfaces
{
    public interface IOrderService
    {
        void CreateOrder(CreateOrderDto orderDto);
        Task<List<OrderDto>> GetOrders();
        Task<bool> DeleteOrder(int id);
        Task<bool> UpdateOrder(int id, OrderUpdateDto updatedOrder);
        Task<MemoryStream> GetOrdersReportExcel(DateTime date);
    }
}
