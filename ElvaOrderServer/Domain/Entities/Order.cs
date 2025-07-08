using ElvaOrderServer.Domain.Exceptions;

namespace ElvaOrderServer.Domain.Entities
{
    public class Order
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string CustomerName { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();

        public int TotalQuantity => Items.Sum(item => item.Quantity);
        public void AddItem(OrderItem item)
        {
            if (item.Quantity <= 0)
            {
                throw new DomainException("Quantity must be greater than zero");
            }

            if (Items.Any(i => i.ProductId == item.ProductId))
            {
                throw new DomainException($"Product {item.ProductId} already exists in the order");
            }

            Items.Add(item);
        }
    }

}
