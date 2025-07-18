using AutoMapper;
using ElvaOrderServer.Application.DTOs;
using ElvaOrderServer.Application.Services;
using ElvaOrderServer.Domain.Constants;
using ElvaOrderServer.Domain.Entities;
using ElvaOrderServer.Domain.Exceptions;
using ElvaOrderServer.Infrastructure.Repositories;
using Microsoft.Extensions.Logging;
using Moq;


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
            CustomerId = 1,
            Items = new List<OrderItemDto>
            {
                new() { ProductId = Guid.NewGuid(), Quantity = 2 },
                new() { ProductId = Guid.NewGuid(), Quantity = 3 }
            }
        };
        
        var order = new Order
        {
            CustomerId = 1,
            Items = request.Items.Select(dto => new OrderItem
            {
                ProductId = dto.ProductId,
                Quantity = dto.Quantity
            }).ToList()
        };

        Random random = new Random();
        order.OrderId = random.Next(1, 199999);

        _mockMapper.Setup(m => m.Map<Order>(request)).Returns(order);

        // Act
        var response = await _service.CreateOrderAsync(request);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.OrderId > 0);       

    }


    [Fact]
    public async Task CreateOrderAsync_WithNonExistingProductId_Should_ThrowException()
    {
        // Arrange
        var nonExistGuid = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa4");
        var request = new CreateOrderRequest
        {
            CustomerId = 1,
            Items = new List<OrderItemDto>
            {
                new() { ProductId = nonExistGuid, Quantity = 1 }
            }
        };

        var order = new Order
        {
            CustomerId = request.CustomerId,
            Items = request.Items.Select(dto => new OrderItem
            {
                ProductId = dto.ProductId,
                Quantity = dto.Quantity
            }).ToList()
        };

        _mockMapper.Setup(m => m.Map<Order>(request)).Returns(order);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<AppException>(() => _service.CreateOrderAsync(request));
        Assert.Equal(ErrorTypes.NotFound, exception.ErrorType);
    }

    [Fact]
    public async Task CreateOrderAsync_WithDuplicateProductIds_Should_ThrowException()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var request = new CreateOrderRequest
        {
            CustomerId = 1,
            Items = new List<OrderItemDto>
            {
                new() { ProductId = productId, Quantity = 1 },
                new() { ProductId = productId, Quantity = 2 }
            }
        };

        var order = new Order
        {
            CustomerId = request.CustomerId,
            Items = request.Items.Select(dto => new OrderItem
            {
                ProductId = dto.ProductId,
                Quantity = dto.Quantity
            }).ToList()
        };

        _mockMapper.Setup(m => m.Map<Order>(request)).Returns(order);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<AppException>(() => _service.CreateOrderAsync(request));
        Assert.Equal(ErrorTypes.InvalidParameter, exception.ErrorType);
    }

    [Fact]
    public async Task GetOrderByIdAsync_WithInvalidId_Should_ThrowException()
    {
        // Arranges
        var invalidId = -1;
        _mockRepo.Setup(r => r.GetByOrderIdAsync(-1)).ReturnsAsync((Order)null);

        // Act & Assert
        var exception =  await Assert.ThrowsAsync<AppException>(() => _service.GetOrderByOrderIdAsync(invalidId));
        Assert.Equal(ErrorTypes.NotFound, exception.ErrorType);
    }
}