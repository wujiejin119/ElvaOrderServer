using ElvaOrderServer.API.Controllers;
using ElvaOrderServer.Domain.Constants;
using ElvaOrderServer.Domain.Entities;
using ElvaOrderServer.Domain.Exceptions;
using ElvaOrderServer.Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
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
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx)
            {
                if (sqlEx.Number == 2601 || sqlEx.Number == 2627)
                {
                    _logger.LogError(ex, "Duplicate key in SQL");

                    throw new InfrastructureException("Duplicate key", ErrorTypes.InvalidParameter);
                }
                throw;
            }
            catch (Exception ex){
                _logger.LogError(ex, "Error adding order on infrastructure");
                throw new InfrastructureException("Faied to save order in repository", ErrorTypes.General);              
            }
            
        }

        public async Task<Order> GetByOrderIdAsync(long orderId)
        {
            return await _context.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);
        }
    }
}
