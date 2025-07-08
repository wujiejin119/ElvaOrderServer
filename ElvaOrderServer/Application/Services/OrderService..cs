using AutoMapper;
using ElvaOrderServer.Application.DTOs;
using ElvaOrderServer.Domain.Entities;
using ElvaOrderServer.Domain.Exceptions;
using ElvaOrderServer.Infrastructure.Repositories;

namespace ElvaOrderServer.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<OrderService> _logger;

        public OrderService(
            IOrderRepository orderRepository,
            IMapper mapper,
            ILogger<OrderService> logger)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<CreateOrderResponse> CreateOrderAsync(CreateOrderRequest request)
        {
            try
            {
                var order = _mapper.Map<Order>(request);
                await _orderRepository.AddAsync(order);

                _logger.LogInformation("Order created: {OrderId}", order.Id);
                return new CreateOrderResponse { OrderId = order.Id };
            }
            catch (DomainException ex)
            {
                _logger.LogWarning("Domain validation failed: {Message}", ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating order");
                throw;
            }
        }

        public async Task<OrderDto> GetOrderByIdAsync(Guid id)
        {
            var order = await _orderRepository.GetByIdAsync(id);

            if (order is null)
            {
                _logger.LogWarning("Order not found: {OrderId}", id);
                throw new OrderNotFoundException(id);
            }

            return _mapper.Map<OrderDto>(order);
        }
    }
}
