using MediatR;
using AvanadeAwesomeShop.Service.Orders.Application.Commands;
using AvanadeAwesomeShop.Service.Orders.Application.Dtos;
using AvanadeAwesomeShop.Service.Orders.Domain.Entities;
using AvanadeAwesomeShop.Service.Orders.Domain.Repositories;
using AvanadeAwesomeShop.Service.Orders.Domain.Enums;
using AvanadeAwesomeShop.Service.Orders.Application.Services;

namespace AvanadeAwesomeShop.Service.Orders.Application.Commands.Handlers
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, OrderDto>
    {
    private readonly IOrderRepository _orderRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IStockNotificationService _stockNotificationService;
    private readonly IStockService _stockService;

        public CreateOrderCommandHandler(
            IOrderRepository orderRepository,
            ICustomerRepository customerRepository,
            IStockNotificationService stockNotificationService,
            IStockService stockService)
        {
            _orderRepository = orderRepository;
            _customerRepository = customerRepository;
            _stockNotificationService = stockNotificationService;
            _stockService = stockService;
        }

        public async Task<OrderDto> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            // Validar se o cliente existe
            var customer = await _customerRepository.GetByEmailAsync(request.OrderDto.CustomerEmail);
            if (customer == null)
            {
                throw new InvalidOperationException($"Customer with email {request.OrderDto.CustomerEmail} not found. Please create the customer first.");
            }

            // Validar estoque dos produtos antes de criar o pedido
            var stockChecks = request.OrderDto.Items
                .Select(item => _stockService.HasStockAsync(item.ProductId, item.Quantity))
                .ToArray();
            var stockResults = await Task.WhenAll(stockChecks);
            bool hasAllStock = stockResults.All(x => x);

            // Criar o pedido (status Started)
            var order = new Order(customer.Id);
            foreach (var itemDto in request.OrderDto.Items)
            {
                order.AddItem(itemDto.ProductId, itemDto.Quantity, itemDto.Price);
            }
            order.CreateOrder();

            if (hasAllStock)
            {
                order.ConfirmOrder();
            }
            else
            {
                order.CancelOrder("Estoque insuficiente para um ou mais itens do pedido.");
            }

            var createdOrder = await _orderRepository.CreateAsync(order);

            // Só notifica redução de estoque se confirmado
            if (createdOrder.Status == OrderStatus.Completed)
            {
                foreach (var item in createdOrder.Items)
                {
                    await _stockNotificationService.NotifyStockReductionAsync(
                        item.ProductId,
                        item.Quantity,
                        createdOrder.Id);
                }
            }

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
