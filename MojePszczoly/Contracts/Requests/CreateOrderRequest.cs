using System.ComponentModel.DataAnnotations;
using MojePszczoly.Contracts.Dtos;

namespace MojePszczoly.Contracts.Requests
{
    public class CreateOrderRequest
    {
        [Required(ErrorMessage = "Customer Name is required")]
        [MinLength(3, ErrorMessage = "Customer Name must be at least 3 characters")]
        public required string CustomerName { get; set; }

        public string? Note { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        public int Phone { get; set; }

        [Required(ErrorMessage = "Order date is required")]
        public DateOnly OrderDate { get; set; }

        [MinLength(1, ErrorMessage = "At least one item is required")]
        public List<OrderItemDto> Items { get; set; } = new();
    }
}
