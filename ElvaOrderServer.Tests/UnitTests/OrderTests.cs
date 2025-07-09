// Tests/UnitTests/OrderTests.cs
using ElvaOrderServer.Domain.Entities;
using ElvaOrderServer.Domain.Exceptions;
using Xunit;

public class OrderTests
{
    [Fact]
    public void AddItem_Should_IncreaseTotalItems()
    {
        // Arrange
        var order = new Order();
        var item = new OrderItem { ProductId = Guid.NewGuid(), Quantity = 2 };
        
        // Act
        order.AddItem(item);
        
        // Assert
        Assert.Single(order.Items);
        Assert.Equal(2, order.TotalQuantity);
    }
    
    [Fact]
    public void AddItem_WithNegativeQuantity_Should_Throw()
    {
        // Arrange
        var order = new Order();
        var item = new OrderItem { ProductId = Guid.NewGuid(), Quantity = -1 };
        
        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => order.AddItem(item));
    }
    
    [Fact]
    public void AddItem_WithZeroQuantity_Should_Throw()
    {
        // Arrange
        var order = new Order();
        var item = new OrderItem { ProductId = Guid.NewGuid(), Quantity = 0 };
        
        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => order.AddItem(item));
    }
    
    [Fact]
    public void AddItem_Should_AddMultipleItems()
    {
        // Arrange
        var order = new Order();
        var item1 = new OrderItem { ProductId = Guid.NewGuid(), Quantity = 1 };
        var item2 = new OrderItem { ProductId = Guid.NewGuid(), Quantity = 3 };
        
        // Act
        order.AddItem(item1);
        order.AddItem(item2);
        
        // Assert
        Assert.Equal(2, order.Items.Count);
        Assert.Equal(4, order.TotalQuantity);
    }
}