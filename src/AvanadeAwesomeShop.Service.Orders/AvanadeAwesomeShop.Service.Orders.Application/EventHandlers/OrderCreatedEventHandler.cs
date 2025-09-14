using MediatR;
using Microsoft.Extensions.Logging;
using AvanadeAwesomeShop.Service.Orders.Domain.Events;
using AvanadeAwesomeShop.Service.Orders.Domain.Repositories;
using AvanadeAwesomeShop.Service.Orders.Application.Services;
using AvanadeAwesomeShop.Service.Orders.Application.Events;

namespace AvanadeAwesomeShop.Service.Orders.Application.EventHandlers
{
    public class OrderCreatedEventHandler : INotificationHandler<DomainEventNotification<OrderCreatedEvent>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IStockService _stockService;
        private readonly ILogger<OrderCreatedEventHandler> _logger;

        public OrderCreatedEventHandler(
            IOrderRepository orderRepository,
            IStockService stockService,
            ILogger<OrderCreatedEventHandler> logger)
        {
            _orderRepository = orderRepository;
            _stockService = stockService;
            _logger = logger;
        }

        public async Task Handle(DomainEventNotification<OrderCreatedEvent> notification, CancellationToken cancellationToken)
        {
            var orderCreatedEvent = notification.DomainEvent;
            _logger.LogInformation("Processing OrderCreatedEvent for Order {OrderId}", orderCreatedEvent.AggregateId);

            try
            {
                // 1. Buscar o pedido criado
                var order = await _orderRepository.GetByIdAsync(orderCreatedEvent.AggregateId);
                if (order == null)
                {
                    _logger.LogError("Order {OrderId} not found", orderCreatedEvent.AggregateId);
                    return;
                }

                // 2. Validar estoque para todos os itens
                _logger.LogInformation("Validating stock for {ItemCount} items in order {OrderId}", order.Items.Count, orderCreatedEvent.AggregateId);
                
                var stockValidationTasks = order.Items.Select(async item => 
                {
                    _logger.LogInformation("Checking stock for ProductId: {ProductId}, Quantity: {Quantity}", item.ProductId, item.Quantity);
                    var hasStock = await _stockService.HasStockAsync(item.ProductId, item.Quantity);
                    _logger.LogInformation("Stock check result for ProductId {ProductId}: {HasStock}", item.ProductId, hasStock);
                    return new
                    {
                        Item = item,
                        HasStock = hasStock
                    };
                });

                var stockValidations = await Task.WhenAll(stockValidationTasks);
                var itemsWithoutStock = stockValidations.Where(v => !v.HasStock).ToList();

                // 3. Decidir status baseado na validação
                if (itemsWithoutStock.Any())
                {
                    var productIds = string.Join(", ", itemsWithoutStock.Select(i => i.Item.ProductId));
                    order.CancelOrder($"Estoque insuficiente para os produtos: {productIds}");
                    _logger.LogWarning("Order {OrderId} cancelled due to insufficient stock", orderCreatedEvent.AggregateId);
                }
                else
                {
                    order.ConfirmOrder();
                    _logger.LogInformation("Order {OrderId} confirmed successfully", orderCreatedEvent.AggregateId);
                }

                // 4. Salvar mudanças (vai disparar OrderConfirmedEvent ou OrderCancelledEvent)
                await _orderRepository.UpdateAsync(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing OrderCreatedEvent for Order {OrderId}", orderCreatedEvent.AggregateId);
                
                // Em caso de erro, cancelar o pedido
                var order = await _orderRepository.GetByIdAsync(orderCreatedEvent.AggregateId);
                if (order != null)
                {
                    order.CancelOrder($"Erro no processamento: {ex.Message}");
                    await _orderRepository.UpdateAsync(order);
                }
            }
        }
    }
}