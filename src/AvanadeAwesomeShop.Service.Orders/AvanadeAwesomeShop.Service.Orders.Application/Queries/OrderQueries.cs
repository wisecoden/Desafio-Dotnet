using MediatR;
using AvanadeAwesomeShop.Service.Orders.Application.Dtos;
using AvanadeAwesomeShop.Service.Orders.Domain.Enums;

namespace AvanadeAwesomeShop.Service.Orders.Application.Queries
{
    public record GetOrderByIdQuery(Guid OrderId) : IRequest<OrderDto?>;
    
    public record GetAllOrdersQuery() : IRequest<IEnumerable<OrderDto>>;
    
    public record GetOrdersByCustomerQuery(Guid CustomerId) : IRequest<IEnumerable<OrderDto>>;
    
    public record GetOrdersByStatusQuery(OrderStatus Status) : IRequest<IEnumerable<OrderDto>>;
    
    public record GetCustomerByIdQuery(Guid CustomerId) : IRequest<CustomerDto?>;
    
    public record GetCustomerByEmailQuery(string Email) : IRequest<CustomerDto?>;
    
    public record GetAllCustomersQuery() : IRequest<IEnumerable<CustomerDto>>;
}
