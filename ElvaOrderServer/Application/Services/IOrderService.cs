using ElvaOrderServer.Application.DTOs;

namespace ElvaOrderServer.Application.Services
{
    public interface IOrderService
    {
        Task<CreateOrderResponse> CreateOrderAsync(CreateOrderRequest request);
        Task<OrderDto> GetOrderByOrderIdAsync(long id);

    }
}
