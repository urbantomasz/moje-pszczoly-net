using AutoMapper;
using MojePszczoly.Contracts.Dtos;
using MojePszczoly.Contracts.Requests;
using MojePszczoly.Contracts.Responses;
using MojePszczoly.Infrastructure.Entities;
using MojePszczoly.Interfaces;
using MojePszczoly.Interfaces.Repositories;

namespace MojePszczoly.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IDateService _dateService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public OrderService(IOrderRepository orderRepository, IDateService dateService, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _orderRepository = orderRepository;
            _dateService = dateService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task CreateOrder(CreateOrderRequest orderDto)
        {
            var order = _mapper.Map<Order>(orderDto);
            await _orderRepository.AddAsync(order);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<OrderResponse>> GetOrders(DateOnly date)
        {
            var orderEntities = await _orderRepository.GetByDateAsync(date);
            return _mapper.Map<List<OrderResponse>>(orderEntities);
        }

        public async Task<List<OrderResponse>> GetOrders()
        {
            return await GetOrdersInternal(isCurrent: true);
        }

        public async Task<List<OrderResponse>> GetPastOrders()
        {
            return await GetOrdersInternal(isCurrent: false);
        }

        private async Task<List<OrderResponse>> GetOrdersInternal(bool isCurrent)
        {
            var currentWeekMonday = _dateService.GetCurrentWeekMonday();
            var orderEntities = isCurrent
                ? await _orderRepository.GetCurrentAsync(currentWeekMonday)
                : await _orderRepository.GetPastAsync(currentWeekMonday);

            return _mapper.Map<List<OrderResponse>>(orderEntities);
        }

        public async Task<bool> DeleteOrder(int id)
        {
            var order = await _orderRepository.GetByIdWithItemsAsync(id);

            if (order == null)
                return false;

            await _orderRepository.DeleteAsync(order);
            await _unitOfWork.SaveChangesAsync();
         
            return true;
        }

        public async Task<bool> UpdateOrder(int id, UpdateOrderRequest updatedOrder)
        {
            var existingOrder = await _orderRepository.GetByIdWithItemsAsync(id);

            if (existingOrder == null)
                return false;

            _mapper.Map(updatedOrder, existingOrder);
            var updatedItems = updatedOrder.Items
                .Select(item => new OrderItem
                {
                    BreadId = item.BreadId,
                    Quantity = item.Quantity,
                    OrderId = id
                })
                .ToList();

            await _orderRepository.ReplaceItemsAsync(existingOrder, updatedItems);
            await _unitOfWork.SaveChangesAsync();
          
            return true;
        }
    }   
}
