using ElvaOrderServer.API.Controllers;
using ElvaOrderServer.Domain.Constants;
using ElvaOrderServer.Domain.Entities;
using ElvaOrderServer.Domain.Exceptions;
using ElvaOrderServer.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ElvaOrderServer.Infrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly OrdersDbContext _context;
        private readonly ILogger<OrdersController> _logger;

        public OrderRepository(OrdersDbContext context, ILogger<OrdersController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task AddAsync(Order order)
        {
            try
            {                
                await _context.Orders.AddAsync(order);
                await _context.SaveChangesAsync();
            }catch (Exception ex){
                _logger.LogError(ex, "Error adding order on infrastructure");
                throw new InfrastructureException("Faied to save order in repository.", ErrorTypes.NotFound);              
            }
            
        }

        public async Task<Order> GetByIdAsync(Guid id)
        {
            return await _context.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == id);
        }
    }
}
