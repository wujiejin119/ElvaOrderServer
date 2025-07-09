using ElvaOrderServer.API.Controllers;
using ElvaOrderServer.Domain.Constants;
using ElvaOrderServer.Domain.Entities;
using ElvaOrderServer.Domain.Exceptions;
using ElvaOrderServer.Infrastructure.Persistence;
using IdGen;
using Microsoft.EntityFrameworkCore;
using Snowflake.Net;

namespace ElvaOrderServer.Infrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly OrdersDbContext _context;
        private readonly ILogger<OrdersController> _logger;
        private readonly IdWorker _idWorker;

        public OrderRepository(OrdersDbContext context, ILogger<OrdersController> logger, IdWorker idWorker)
        {
            _context = context;
            _logger = logger; 
            _idWorker = idWorker;
        }

        public async Task AddAsync(Order order)
        {
            try
            {
                order.OrderId = _idWorker.NextId();
                await _context.Orders.AddAsync(order);
                await _context.SaveChangesAsync();
            }catch (Exception ex){
                _logger.LogError(ex, "Error adding order on infrastructure");
                throw new InfrastructureException("Faied to save order in repository.", ErrorTypes.General);              
            }
            
        }

        public async Task<Order> GetByOrderIdAsync(long id)
        {
            return await _context.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == id);
        }
    }
}
