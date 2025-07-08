using AutoMapper;
using ElvaOrderServer.Application.DTOs;
using ElvaOrderServer.Domain.Entities;

namespace ElvaOrderServer.Application.Services
{
    public class OrderProfile : Profile
    {
        public OrderProfile()
        {
            CreateMap<CreateOrderRequest, Order>();
            CreateMap<OrderItemDto, OrderItem>();
            CreateMap<Order, OrderDto>();
            CreateMap<OrderItem, OrderItemDto>();
        }
    }
}
