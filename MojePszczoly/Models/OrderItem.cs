namespace MojePszczoly.Models
{
    public class OrderItem
    {
        public int OrderItemId { get; set; }
        public int BreadId { get; set; }
        public int Quantity { get; set; }
        public Bread Bread { get; set; }

    }
}
