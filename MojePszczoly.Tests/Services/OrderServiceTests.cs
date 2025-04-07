using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using MojePszczoly.Data;
using MojePszczoly.Data.Models;
using MojePszczoly.Interfaces;
using MojePszczoly.Models;
using MojePszczoly.Services;
using Moq;

public class OrderServiceTests
{
    private readonly Mock<AppDbContext> _mockContext;
    private readonly Mock<IDateService> _mockDateService; 
    private readonly Mock<IMemoryCache> _mockMemoryCache;
    private readonly IOrderService _orderService; 

    public OrderServiceTests()
    {
        var options = new DbContextOptions<AppDbContext>();
        _mockContext = new Mock<AppDbContext>(options);
        _mockDateService = new Mock<IDateService>();
        _mockMemoryCache = new Mock<IMemoryCache>();
        _orderService = new OrderService(_mockContext.Object, _mockDateService.Object, _mockMemoryCache.Object);
    }

    [Fact]
    public void CreateOrder_AddsOrderToContext()
    {
        // Arrange
        var orderDto = new CreateOrderDto
        {
            CustomerName = "John Doe",
            Phone = 123456789,
            OrderDate = DateTime.UtcNow,
            Note = "Test note",
            Items = new List<CreateOrderItemDto>
            {
                new CreateOrderItemDto { BreadId = 1, Quantity = 2 }
            }
        };

        var mockSet = new Mock<DbSet<Order>>();
        _mockContext.Setup(m => m.Orders).Returns(mockSet.Object);

        // Act
        _orderService.CreateOrder(orderDto);

        // Assert
        mockSet.Verify(m => m.Add(It.IsAny<Order>()), Times.Once());
        _mockContext.Verify(m => m.SaveChanges(), Times.Once());
    }

    [Fact]
    public async Task UpdateOrder_UpdatesOrderInContext()
    {
        // Arrange
        var order = new Order { OrderId = 1, CustomerName = "John Doe", Items = new List<OrderItem>() };
        var updatedOrder = new OrderUpdateDto
        {
            CustomerName = "Jane Doe",
            Phone = 987654321,
            OrderDate = DateTime.UtcNow,
            Note = "Updated note",
            Items = new List<OrderItemDto>
            {
                new OrderItemDto { BreadId = 1, Quantity = 3 }
            }
        };

        var mockSet = new Mock<DbSet<Order>>();
        mockSet.Setup(m => m.FindAsync(1)).ReturnsAsync(order);
        _mockContext.Setup(m => m.Orders).Returns(mockSet.Object);

        // Act
        var result = await _orderService.UpdateOrder(1, updatedOrder);

        // Assert
        Assert.True(result);
        Assert.Equal("Jane Doe", order.CustomerName);
        Assert.Equal(987654321, order.Phone);
        Assert.Equal("Updated note", order.Note);
        mockSet.Verify(m => m.Update(It.IsAny<Order>()), Times.Once());
        _mockContext.Verify(m => m.SaveChangesAsync(default), Times.Once());
    }

}
