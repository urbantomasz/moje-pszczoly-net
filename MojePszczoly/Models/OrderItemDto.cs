namespace MojePszczoly.Models
{
    public class OrderItemDto
    {
        public int OrderItemId { get; set; }
        public int BreadId { get; set; }
        public string BreadName { get; set; } // Można dodać nazwę chleba od razu
        public int Quantity { get; set; }
    }
}
