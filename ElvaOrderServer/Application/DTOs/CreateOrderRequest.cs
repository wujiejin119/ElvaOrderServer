using System.ComponentModel.DataAnnotations;

namespace ElvaOrderServer.Application.DTOs
{
    public class CreateOrderRequest
    {
        [Required(ErrorMessage = "Customer name is required")]
        [StringLength(100, MinimumLength = 2,
            ErrorMessage = "Customer name must be between 2-100 characters")]
        public string CustomerName { get; set; } = null!;

        [Required(ErrorMessage = "At least one order item is required")]
        [MinLength(1, ErrorMessage = "At least one order item is required")]
        [MaxLength(10, ErrorMessage = "Maximum 10 items per order")]
        public List<OrderItemDto> Items { get; set; } = new List<OrderItemDto>();
    }
}
