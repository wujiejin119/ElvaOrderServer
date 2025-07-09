namespace ElvaOrderServer.Domain.Entities
{
    public class OrderItem
    {
        public long Id { get; set; }
        public Guid ProductId { get; set; }
        public decimal Quantity { get; set; }

        public long OrderId { get; set; }
        public Order Order { get; set; } = null!;
    }
}
