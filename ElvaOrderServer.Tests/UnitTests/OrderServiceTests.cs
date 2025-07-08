using AutoMapper;
using ElvaOrderServer.Application.DTOs;
using ElvaOrderServer.Application.Services;
using ElvaOrderServer.Domain.Entities;
using ElvaOrderServer.Infrastructure.Repositories;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

public class OrderServiceTests
{
    private readonly Mock<IOrderRepository> _mockRepo;
    private readonly Mock<IMapper> _mockMapper;
    private readonly OrderService _service;
    
    public OrderServiceTests()
    {
        _mockRepo = new Mock<IOrderRepository>();
        _mockMapper = new Mock<IMapper>();
        _service = new OrderService(
            _mockRepo.Object, 
            _mockMapper.Object, 
            Mock.Of<ILogger<OrderService>>());
    }

    [Fact]
    public async Task CreateOrderAsync_ValidRequest_Should_Succeed()
    {
        // Arrange
        var request = new CreateOrderRequest 
        { 
            CustomerName = "Test Customer",
            Items = new List<OrderItemDto>
            {
                new() { ProductId = Guid.NewGuid(), Quantity = 2 },
                new() { ProductId = Guid.NewGuid(), Quantity = 3 }
            }
        };
        
        var order = new Order
        {
            CustomerName = request.CustomerName,
            Items = request.Items.Select(dto => new OrderItem
            {
                ProductId = dto.ProductId,
                Quantity = dto.Quantity
            }).ToList()
        };
        
        // ÉèÖÃÓ³ÉäÆ÷ÐÐÎª
        _mockMapper.Setup(m => m.Map<Order>(request)).Returns(order);
        
        // Act
        var response = await _service.CreateOrderAsync(request);
        
        // Assert
        Assert.NotNull(response);
        _mockRepo.Verify(r => r.AddAsync(It.Is<Order>(o => 
            o.CustomerName == request.CustomerName &&
            o.Items.Count == request.Items.Count &&
            o.Items.Sum(i => i.Quantity) == 5 // 2 + 3
        )), Times.Once);
    }
    
    [Fact]
    public async Task CreateOrderAsync_WithItems_Should_MapCorrectly()
    {
        // Arrange
        var productId1 = Guid.NewGuid();
        var productId2 = Guid.NewGuid();
        
        var request = new CreateOrderRequest 
        { 
            CustomerName = "Mapping Test",
            Items = new List<OrderItemDto>
            {
                new() { ProductId = productId1, Quantity = 1 },
                new() { ProductId = productId2, Quantity = 4 }
            }
        };
        
        var order = new Order();
        _mockMapper.Setup(m => m.Map<Order>(request)).Returns(order);
        
        // Act
        await _service.CreateOrderAsync(request);
        
        // Assert
        _mockMapper.Verify(m => m.Map<Order>(request), Times.Once);
    }
    
    [Fact]
    public async Task CreateOrderAsync_Should_ReturnOrderId()
    {
        // Arrange
        var request = new CreateOrderRequest 
        { 
            CustomerName = "Order ID Test",
            Items = new List<OrderItemDto>
            {
                new() { ProductId = Guid.NewGuid(), Quantity = 3 }
            }
        };
        
        var expectedOrderId = Guid.NewGuid();
        var order = new Order { Id = expectedOrderId };
        
        _mockMapper.Setup(m => m.Map<Order>(request)).Returns(order);
        
        // Act
        var response = await _service.CreateOrderAsync(request);
        
        // Assert
        Assert.NotNull(response);
        Assert.Equal(expectedOrderId, response.OrderId);
    }
}