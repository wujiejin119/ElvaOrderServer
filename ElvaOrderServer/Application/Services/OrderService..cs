using AutoMapper;
using ElvaOrderServer.Application.DTOs;
using ElvaOrderServer.Domain.Constants;
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
                Order order = _mapper.Map<Order>(request);
                await _orderRepository.AddAsync(order);

                validateFields(order);

                _logger.LogInformation("Order created: {OrderId}, ExternalId: {ExternalId}",
                    order.OrderId, order.ExternalOrderId);

                return new CreateOrderResponse { OrderId = order.OrderId };
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

        public async Task<OrderDto> GetOrderByOrderIdAsync(long orderId)
        {
            var order = await _orderRepository.GetByOrderIdAsync(orderId);

            if (order is null)
            {
                _logger.LogWarning("Order not found: {OrderId}", orderId);
                throw new AppException("Order not found", ErrorTypes.NotFound);
            }

            return _mapper.Map<OrderDto>(order);
        }

        private void validateFields(Order order) {
            Guid nonExistGuid = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa4");
            // Should connect DB but use a non-exist guid just for demo
            // if (!await _customerRepository.ExistsAsync(customerId))
            //if (!await _productRepository.ExistsAsync(item.ProductId))


            if (order.Items.Any(item => item.ProductId.Equals(nonExistGuid) || item.ProductId.Equals(Guid.Empty) ))// not 00000000-0000-0000-0000-000000000000
            {
                _logger.LogError("Product not found");
                throw new AppException("Product not found", ErrorTypes.NotFound);
            }

            var hasDuplicates = order.Items
                .GroupBy(item => item.ProductId)
                .Any(group => group.Count() > 1);

            if (hasDuplicates)
            {
                _logger.LogError("duplicate product");
                throw new AppException("Duplicate product IDs are not allowed", ErrorTypes.InvalidParameter);
            }

        }
    }
}
