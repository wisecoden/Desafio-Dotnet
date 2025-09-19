using MediatR;
using AvanadeAwesomeShop.Service.Orders.Application.Commands;
using AvanadeAwesomeShop.Service.Orders.Application.Dtos;
using AvanadeAwesomeShop.Service.Orders.Domain.Entities;
using AvanadeAwesomeShop.Service.Orders.Domain.Repositories;
using AvanadeAwesomeShop.Service.Orders.Domain.Enums;

namespace AvanadeAwesomeShop.Service.Orders.Application.Commands.Handlers
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, OrderDto>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICustomerRepository _customerRepository;

        public CreateOrderCommandHandler(
            IOrderRepository orderRepository,
            ICustomerRepository customerRepository)
        {
            _orderRepository = orderRepository;
            _customerRepository = customerRepository;
        }

        public async Task<OrderDto> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            // Validar se o cliente existe
            var customer = await _customerRepository.GetByIdAsync(request.OrderDto.CustomerId);
            if (customer == null)
            {
                throw new InvalidOperationException($"Customer with ID {request.OrderDto.CustomerId} not found. Please create the customer first.");
            }

            // Criar o pedido com status "Started" - validação de estoque será assíncrona
            var order = new Order(customer.Id);
            foreach (var itemDto in request.OrderDto.Items)
            {
                order.AddItem(itemDto.ProductId, itemDto.Quantity, itemDto.Price);
            }
            
            // Apenas criar - não validar estoque aqui (será feito pelo event handler)
            order.CreateOrder();

            // Salvar o pedido - isso vai disparar o OrderCreatedEvent
            var createdOrder = await _orderRepository.CreateAsync(order);

            return MapToOrderDto(createdOrder);
        }

        private static OrderDto MapToOrderDto(Order order)
        {
            return new OrderDto
            {
                Id = order.Id,
                CustomerId = order.CustomerId,
                Status = order.Status,
                StatusDescription = GetStatusDescription(order.Status),
                TotalPrice = order.TotalPrice,
                CreatedAt = order.CreatedAt,
                UpdatedAt = order.UpdatedAt,
                Items = order.Items.Select(item => new OrderItemDto
                {
                    Id = item.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = item.Price,
                }).ToList()
            };
        }

        private static string GetStatusDescription(OrderStatus status)
        {
            return status switch
            {
                OrderStatus.Started => "Iniciado",
                OrderStatus.Completed => "Completado",
                OrderStatus.Rejected => "Rejeitado",
                _ => "Desconhecido"
            };
        }
    }
}
