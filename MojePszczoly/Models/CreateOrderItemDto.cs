using System.ComponentModel.DataAnnotations;

namespace MojePszczoly.Models
{
    public class CreateOrderItemDto
    {
        [Required(ErrorMessage = "BreadId is required")]
        public int BreadId { get; set; }

        [Range(1, 50, ErrorMessage = "Quantity must be between 1 and 50")]
        public int Quantity { get; set; }
    }
}
