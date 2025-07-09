using System.ComponentModel.DataAnnotations;

namespace ElvaOrderServer.Application.DTOs
{
    public class OrderItemDto
    {
        [Required(ErrorMessage = "Product ID is required")]
        [Range(typeof(Guid), "00000000-0000-0000-0000-000000000001", "ffffffff-ffff-ffff-ffff-ffffffffffff", ErrorMessage = "Product ID cannot be empty")]
        public Guid ProductId { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Quantity must be greater than zero")]
        public decimal Quantity { get; set; }
    }
}
