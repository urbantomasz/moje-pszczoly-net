using MojePszczoly.Contracts.Dtos;

namespace MojePszczoly.Contracts.Responses
{
    public class OrderResponse
    {
        public int OrderId { get; set; }
        public required string CustomerName { get; set; }
        public string? Note { get; set; }
        public int Phone { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateOnly OrderDate { get; set; }
        public required List<OrderItemDto> Items { get; set; }
    }

}
