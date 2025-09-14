using MediatR;
using Microsoft.Extensions.Logging;
using AvanadeAwesomeShop.Service.Orders.Domain.Events;
using AvanadeAwesomeShop.Service.Orders.Domain.Repositories;
using AvanadeAwesomeShop.Service.Orders.Application.Services;
using AvanadeAwesomeShop.Service.Orders.Application.Events;

namespace AvanadeAwesomeShop.Service.Orders.Application.EventHandlers
{
    public class OrderConfirmedEventHandler : INotificationHandler<DomainEventNotification<OrderConfirmedEvent>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IStockNotificationService _stockNotificationService;
        private readonly ILogger<OrderConfirmedEventHandler> _logger;

        public OrderConfirmedEventHandler(
            IOrderRepository orderRepository,
            IStockNotificationService stockNotificationService,
            ILogger<OrderConfirmedEventHandler> logger)
        {
            _orderRepository = orderRepository;
            _stockNotificationService = stockNotificationService;
            _logger = logger;
        }

        public async Task Handle(DomainEventNotification<OrderConfirmedEvent> notification, CancellationToken cancellationToken)
        {
            var orderConfirmedEvent = notification.DomainEvent;
            _logger.LogInformation("Processing OrderConfirmedEvent for Order {OrderId}", orderConfirmedEvent.AggregateId);

            try
            {
                // Buscar o pedido confirmado
                var order = await _orderRepository.GetByIdAsync(orderConfirmedEvent.AggregateId);
                if (order == null)
                {
                    _logger.LogError("Order {OrderId} not found", orderConfirmedEvent.AggregateId);
                    return;
                }

                // Notificar redução de estoque para todos os itens
                var notificationTasks = order.Items.Select(item =>
                    _stockNotificationService.NotifyStockReductionAsync(
                        item.ProductId,
                        item.Quantity,
                        order.Id));

                await Task.WhenAll(notificationTasks);

                _logger.LogInformation("Stock reduction notifications sent for Order {OrderId}", orderConfirmedEvent.AggregateId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing OrderConfirmedEvent for Order {OrderId}", orderConfirmedEvent.AggregateId);
                // Não relançar a exceção para não quebrar o fluxo de outros eventos
            }
        }
    }
}