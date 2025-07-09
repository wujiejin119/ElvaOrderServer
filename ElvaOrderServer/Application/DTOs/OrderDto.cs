namespace ElvaOrderServer.Application.DTOs
{
    public class OrderDto
    {    
        public long Id { get; set; }
        public Guid ExternalOrderId { get; set; }
        public long OrderId { get; set; }
        public long CustomerId { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<OrderItemDto> Items { get; set; } = new List<OrderItemDto>();
    }
}
