using AutoMapper;
using MojePszczoly.Contracts.Dtos;
using MojePszczoly.Contracts.Requests;
using MojePszczoly.Contracts.Responses;
using MojePszczoly.Infrastructure.Entities;

namespace MojePszczoly.Infrastructure.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Bread, BreadResponse>();

            CreateMap<OrderItem, OrderItemDto>();

            CreateMap<Order, OrderResponse>()
                .ForMember(
                    destination => destination.CreatedAt,
                    options => options.MapFrom(source => DateTime.SpecifyKind(source.CreatedAt, DateTimeKind.Utc)));

            CreateMap<OrderItemDto, OrderItem>();

            CreateMap<CreateOrderRequest, Order>()
                .ForMember(destination => destination.OrderId, options => options.Ignore())
                .ForMember(destination => destination.CreatedAt, options => options.Ignore())
                .ForMember(destination => destination.UpdatedAt, options => options.Ignore());

            CreateMap<UpdateOrderRequest, Order>()
                .ForMember(destination => destination.OrderId, options => options.Ignore())
                .ForMember(destination => destination.CreatedAt, options => options.Ignore())
                .ForMember(destination => destination.UpdatedAt, options => options.Ignore())
                .ForMember(destination => destination.Items, options => options.Ignore());
        }
    }
}