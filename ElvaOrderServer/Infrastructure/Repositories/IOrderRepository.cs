using ElvaOrderServer.Domain.Entities;

namespace ElvaOrderServer.Infrastructure.Repositories
{
    public interface IOrderRepository
    {
        Task<Order> GetByOrderIdAsync(long id);
        Task AddAsync(Order order);
    }
}
