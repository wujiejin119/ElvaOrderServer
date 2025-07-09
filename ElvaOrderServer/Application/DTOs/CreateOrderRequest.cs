using System.ComponentModel.DataAnnotations;

namespace ElvaOrderServer.Application.DTOs
{
    public class CreateOrderRequest
    {
        [Required(ErrorMessage = "Customer ID is required")]
        [Range(1, long.MaxValue, ErrorMessage = "Customer ID must be greater than 0")]
        public long CustomerId { get; set; }

        [Required(ErrorMessage = "External Order ID is required")]
        [Range(typeof(Guid), "00000000-0000-0000-0000-000000000001", "ffffffff-ffff-ffff-ffff-ffffffffffff", ErrorMessage = "External Order ID cannot be empty")]
        public Guid ExternalOrderId { get; set; }

        [Required(ErrorMessage = "At least one order item is required")]
        [MinLength(1, ErrorMessage = "At least one order item is required")]
        [MaxLength(10, ErrorMessage = "Maximum 10 items per order")]
        public List<OrderItemDto> Items { get; set; } = new List<OrderItemDto>();
    }
}
