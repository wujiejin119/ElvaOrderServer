
using System;
namespace ElvaOrderServer.Domain.Exceptions
{
    public class OrderNotFoundException : Exception
    {
        public Guid OrderId { get; }

        public OrderNotFoundException(Guid orderId)
            : base($"Order with ID {orderId} not found")
        {
            OrderId = orderId;
        }

        public OrderNotFoundException(Guid orderId, Exception innerException)
            : base($"Order with ID {orderId} not found", innerException)
        {
            OrderId = orderId;
        }
    }
}

