namespace ElvaOrderServer.Domain.Entities
{
    public class Order
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string CustomerName { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    }
}
