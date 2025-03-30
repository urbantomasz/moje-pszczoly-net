using Microsoft.EntityFrameworkCore;
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
    private readonly IOrderService _orderService; 

    public OrderServiceTests()
    {
        var options = new DbContextOptions<AppDbContext>();
        _mockContext = new Mock<AppDbContext>(options);
        _mockDateService = new Mock<IDateService>(); 
        _orderService = new OrderService(_mockContext.Object, _mockDateService.Object);
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
}
