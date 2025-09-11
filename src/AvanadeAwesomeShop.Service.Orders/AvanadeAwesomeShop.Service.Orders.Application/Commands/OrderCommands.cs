using MediatR;
using AvanadeAwesomeShop.Service.Orders.Application.Dtos;

namespace AvanadeAwesomeShop.Service.Orders.Application.Commands
{
    public record CreateOrderCommand(CreateOrderDto OrderDto) : IRequest<OrderDto>;
    public record CreateCustomerCommand(CreateCustomerDto CustomerDto) : IRequest<CustomerDto>;
}
