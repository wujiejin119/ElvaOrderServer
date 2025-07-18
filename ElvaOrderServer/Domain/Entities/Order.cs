﻿using ElvaOrderServer.Domain.Exceptions;

namespace ElvaOrderServer.Domain.Entities
{
    public class Order
    {
        public long Id { get; set; }
        public Guid ExternalOrderId { get; set; }
        public long OrderId { get; set; }
        public long CustomerId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();

        public decimal TotalQuantity => Items.Sum(item => item.Quantity);
        public void AddItem(OrderItem item)
        {
            if (item.Quantity <= 0)
            {
                throw new DomainException("Quantity must be greater than zero");
            }
            Items.Add(item);
        }
    }

}
