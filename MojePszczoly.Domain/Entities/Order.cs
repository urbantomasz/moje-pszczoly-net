namespace MojePszczoly.Domain.Entities
{
    public class Order
    {
        public int OrderId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public required string CustomerName { get; set; }
        public string? Note { get; set; }
        public int Phone { get; set; }
        public DateOnly OrderDate { get; set; }
        public List<OrderItem> Items { get; set; } = new();
    }
}
