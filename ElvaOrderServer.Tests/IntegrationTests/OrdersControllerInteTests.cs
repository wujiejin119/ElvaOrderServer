using ElvaOrderServer.Application.DTOs;
using ElvaOrderServer.Domain.Entities;
using ElvaOrderServer.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;

namespace ElvaOrderServer.API.IntegrationTests
{
    public class OrdersControllerInteTests : IClassFixture<WebApplicationFactory<Program>>, IDisposable
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;
        private readonly OrdersDbContext _dbContext;

        public OrdersControllerInteTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<OrdersDbContext>));

                    if (descriptor != null)
                    {
                        services.Remove(descriptor);
                    }

                    services.AddDbContext<OrdersDbContext>(options =>
                    {
                        options.UseInMemoryDatabase("TestDatabase");
                    });
                });
            });

            _client = _factory.CreateClient();
            _dbContext = _factory.Services.CreateScope().ServiceProvider.GetRequiredService<OrdersDbContext>();
        }

        [Fact]
        public async Task CreateOrder_ValidRequest_ShouldReturn201Created()
        {
            // Arrange
            var request = new CreateOrderRequest
            {
                CustomerId = Guid.NewGuid(),
                Items = new List<OrderItemDto>
                {
                    new() { ProductId = Guid.NewGuid(), Quantity = 2 },
                    new() { ProductId = Guid.NewGuid(), Quantity = 3 }
                }
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/orders", request);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var createdOrder = await response.Content.ReadFromJsonAsync<CreateOrderResponse>();
            Assert.NotNull(createdOrder);
            Assert.NotEqual(Guid.Empty, createdOrder.OrderId);

            var dbOrder = await _dbContext.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == createdOrder.OrderId);

            Assert.NotNull(dbOrder);
            Assert.Equal(request.CustomerId, dbOrder.CustomerId);
            Assert.Equal(request.Items.Count, dbOrder.Items.Count);
            Assert.Equal(5, dbOrder.Items.Sum(i => i.Quantity)); // 2 + 3
        }

        [Fact]
        public async Task CreateOrder_InvalidRequestNoItem_ShouldReturn400BadRequest()
        {
            // Arrange
            var invalidRequest = new CreateOrderRequest
            {
                CustomerId = Guid.NewGuid(),
                Items = new List<OrderItemDto>()
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/orders", invalidRequest);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task CreateOrder_InvalidRequestNoCustomer_ShouldReturn400BadRequest()
        {
            // Arrange
            var invalidRequest = new CreateOrderRequest
            {
                CustomerId = Guid.Empty,
                Items = new List<OrderItemDto>
                {
                    new() { ProductId = Guid.NewGuid(), Quantity = 2 },
                    new() { ProductId = Guid.NewGuid(), Quantity = 3 }
                }
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/orders", invalidRequest);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task CreateOrder_DuplicateProductIds_ShouldReturn400BadRequest()
        {
            // Arrange
            var duplicateProductId = Guid.NewGuid();
            var request = new CreateOrderRequest
            {
                CustomerId = Guid.NewGuid(),
                Items = new List<OrderItemDto>
                {
                    new() { ProductId = duplicateProductId, Quantity = 1 },
                    new() { ProductId = duplicateProductId, Quantity = 2 }
                }
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/orders", request);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task GetOrder_ExistingId_ShouldReturn200WithOrder()
        {            
            var testOrder = new Order
            {
                CustomerId = Guid.NewGuid(),
                Items = new List<OrderItem>
                {
                    new OrderItem { ProductId = Guid.NewGuid(), Quantity = 1 }
                }
            };

            _dbContext.Orders.Add(testOrder);
            await _dbContext.SaveChangesAsync();

            // Act
            var response = await _client.GetAsync($"/api/orders/{testOrder.Id}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var order = await response.Content.ReadFromJsonAsync<OrderDto>();
            Assert.NotNull(order);
            Assert.Equal(testOrder.Id, order.Id);
            Assert.Equal(testOrder.CustomerId, order.CustomerId);
            Assert.Equal(testOrder.Items.Count, order.Items.Count);
        }

        [Fact]
        public async Task GetOrder_NonExistingId_ShouldReturn404NotFound()
        {
            // Arrange
            var nonExistingId = Guid.NewGuid();

            // Act
            var response = await _client.GetAsync($"/api/orders/{nonExistingId}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        public void Dispose()
        {
            _dbContext.Database.EnsureDeleted();
            _client.Dispose();
        }
    }
}