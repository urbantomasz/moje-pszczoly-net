namespace MojePszczoly.Models
{
    public class OrderUpdateDto
    {
        public int OrderId { get; set; }
        public string CustomerName { get; set; }
        public string? Note { get; set; }
        public int Phone { get; set; }
        public DateTime OrderDate { get; set; }
        public List<OrderItemDto> Items { get; set; }
    }
}
