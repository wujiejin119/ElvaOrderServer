using System.ComponentModel.DataAnnotations;

namespace ElvaOrderServer.Application.DTOs
{
    public class CreateOrderRequest
    {
        [Required(ErrorMessage = "Customer ID is required")]
        public Guid CustomerId { get; set; }

        [Required(ErrorMessage = "At least one order item is required")]
        [MinLength(1, ErrorMessage = "At least one order item is required")]
        [MaxLength(10, ErrorMessage = "Maximum 10 items per order")]
        public List<OrderItemDto> Items { get; set; } = new List<OrderItemDto>();
    }
}
