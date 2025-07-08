using System.ComponentModel.DataAnnotations;

namespace ElvaOrderServer.Application.DTOs
{
    public class OrderItemDto
    {
        [Required(ErrorMessage = "Product ID is required")]
        public Guid ProductId { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, 100, ErrorMessage = "Quantity must be between 1-100")]
        public int Quantity { get; set; }
    }
}
