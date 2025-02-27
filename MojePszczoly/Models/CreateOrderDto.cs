using System.ComponentModel.DataAnnotations;

namespace MojePszczoly.Models
{
    public class CreateOrderDto
    {
        [Required(ErrorMessage = "Customer Name is required")]
        [MinLength(3, ErrorMessage = "Customer Name must be at least 3 characters")]
        public string CustomerName { get; set; }

        public string? Note { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        public int Phone { get; set; }

        [Required(ErrorMessage = "Order date is required")]
        public DateTime OrderDate { get; set; }

        [MinLength(1, ErrorMessage = "At least one item is required")]
        public List<CreateOrderItemDto> Items { get; set; } = new();
    }
}
