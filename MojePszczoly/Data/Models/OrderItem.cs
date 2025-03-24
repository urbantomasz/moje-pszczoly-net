 using System.ComponentModel.DataAnnotations;

namespace MojePszczoly.Data.Models
{
    public class OrderItem
    {
        public int OrderItemId { get; set; }
        public int OrderId { get; set; }
        public int BreadId { get; set; }
        public int Quantity { get; set; }
        public Order Order { get; set; }
        public Bread Bread { get; set; }
    }
}
