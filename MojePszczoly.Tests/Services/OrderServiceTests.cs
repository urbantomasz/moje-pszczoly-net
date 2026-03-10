using AutoMapper;
using MojePszczoly.Contracts.Dtos;
using MojePszczoly.Contracts.Requests;
using MojePszczoly.Domain.Entities;
using MojePszczoly.Application.Interfaces;
using MojePszczoly.Application.Interfaces.Repositories;
using MojePszczoly.Application.Services;
using Moq;

namespace MojePszczoly.Tests.Services
{
    public class OrderServiceTests
    {
        private readonly Mock<IOrderRepository> _mockOrderRepository;
        private readonly Mock<IDateService> _mockDateService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly IOrderService _orderService;

        public OrderServiceTests()
        {
            _mockOrderRepository = new Mock<IOrderRepository>();
            _mockDateService = new Mock<IDateService>();
            _mockMapper = new Mock<IMapper>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _orderService = new OrderService(_mockOrderRepository.Object, _mockDateService.Object, _mockMapper.Object, _mockUnitOfWork.Object);
        }

        [Fact]
        public async Task CreateOrder_AddsOrderToRepository()
        {
            var orderDto = new CreateOrderRequest
            {
                CustomerName = "John Doe",
                Phone = 123456789,
                OrderDate = DateOnly.FromDateTime(DateTime.Now),
                Note = "Test note",
                Items = new List<OrderItemDto>
                {
                    new OrderItemDto { BreadId = 1, Quantity = 2 }
                }
            };

            var mappedOrder = new Order
            {
                CustomerName = orderDto.CustomerName,
                Phone = orderDto.Phone,
                OrderDate = orderDto.OrderDate,
                Note = orderDto.Note,
                Items = new List<OrderItem>()
            };

            _mockMapper.Setup(mapper => mapper.Map<Order>(orderDto)).Returns(mappedOrder);

            await _orderService.CreateOrder(orderDto);

            _mockOrderRepository.Verify(repository => repository.AddAsync(mappedOrder), Times.Once());
            _mockUnitOfWork.Verify(unitOfWork => unitOfWork.SaveChangesAsync(default), Times.Once());
        }

        [Fact]
        public async Task UpdateOrder_ReplacesItemsAndSavesChanges()
        {
            var existingOrder = new Order
            {
                OrderId = 1,
                CustomerName = "John Doe",
                Items = new List<OrderItem>
                {
                    new OrderItem { OrderId = 1, BreadId = 1, Quantity = 1 }
                }
            };

            var updatedOrder = new UpdateOrderRequest
            {
                CustomerName = "Jane Doe",
                Phone = 987654321,
                OrderDate = DateOnly.FromDateTime(DateTime.Now),
                Note = "Updated note",
                Items = new List<OrderItemDto>
                {
                    new OrderItemDto { BreadId = 1, Quantity = 3 }
                }
            };

            _mockOrderRepository
                .Setup(repository => repository.GetByIdWithItemsAsync(1))
                .ReturnsAsync(existingOrder);

            _mockMapper
                .Setup(mapper => mapper.Map(updatedOrder, existingOrder))
                .Returns(existingOrder);

            var result = await _orderService.UpdateOrder(1, updatedOrder);

            Assert.True(result);
            _mockMapper.Verify(mapper => mapper.Map(updatedOrder, existingOrder), Times.Once());
            _mockOrderRepository.Verify(
                repository => repository.ReplaceItemsAsync(
                    existingOrder,
                    It.Is<List<OrderItem>>(items => items.Count == 1 && items[0].BreadId == 1 && items[0].Quantity == 3 && items[0].OrderId == 1)),
                Times.Once());
            _mockUnitOfWork.Verify(unitOfWork => unitOfWork.SaveChangesAsync(default), Times.Once());
        }
    }
}
