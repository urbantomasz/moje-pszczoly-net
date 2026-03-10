using Microsoft.Extensions.Caching.Memory;
using MojePszczoly.Contracts.Requests;
using MojePszczoly.Contracts.Responses;
using MojePszczoly.Application.Interfaces;

namespace MojePszczoly.Application.Services;

public class CachedOrderService : IOrderService
{
    private readonly IOrderService _inner;
    private readonly IMemoryCache _cache;

    public CachedOrderService(IOrderService inner, IMemoryCache cache)
    {
        _inner = inner;
        _cache = cache;
    }

    public Task CreateOrder(CreateOrderRequest orderDto)
    {
        _cache.Remove("orders");
        return _inner.CreateOrder(orderDto);
    }

    public Task<bool> DeleteOrder(int id)
    {
        _cache.Remove("orders");
        return _inner.DeleteOrder(id);
    }

    public async Task<List<OrderResponse>> GetOrders()
    {
        if (!_cache.TryGetValue("orders:current", out List<OrderResponse>? orders))
        {
            orders = await _inner.GetOrders();
            _cache.Set("orders:current", orders, TimeSpan.FromMinutes(5));
        }

        return orders!;
    }

    public async Task<List<OrderResponse>> GetOrders(DateOnly date)
    {
        return await _inner.GetOrders(date);
    }

    public async Task<List<OrderResponse>> GetPastOrders()
    {
        if (!_cache.TryGetValue("orders:past", out List<OrderResponse>? orders))
        {
            orders = await _inner.GetPastOrders();
            _cache.Set("orders:past", orders, new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(5)));
        }
        return orders!;
    }

    public Task<bool> UpdateOrder(int id, UpdateOrderRequest updatedOrder)
    {
        _cache.Remove("orders");
        return _inner.UpdateOrder(id, updatedOrder);
    }
}