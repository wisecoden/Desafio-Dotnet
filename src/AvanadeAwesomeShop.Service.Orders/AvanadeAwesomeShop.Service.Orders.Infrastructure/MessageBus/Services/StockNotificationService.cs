using AvanadeAwesomeShop.Service.Orders.Infrastructure.MessageBus.Events;
using AvanadeAwesomeShop.Service.Orders.Infrastructure.MessageBus.Configuration;
using AvanadeAwesomeShop.Service.Orders.Application.Services;
using Microsoft.Extensions.Logging;

namespace AvanadeAwesomeShop.Service.Orders.Infrastructure.MessageBus.Services
{
    public class StockNotificationService : IStockNotificationService
    {
        private readonly IMessagePublisher _messagePublisher;
        private readonly ILogger<StockNotificationService> _logger;

        public StockNotificationService(
            IMessagePublisher messagePublisher, 
            ILogger<StockNotificationService> logger)
        {
            _messagePublisher = messagePublisher;
            _logger = logger;
        }

        public async Task NotifyStockReductionAsync(Guid productId, int quantity, Guid orderId)
        {
            try
            {
                var stockReducedEvent = new StockReducedEvent(productId, quantity, orderId);
                
                // Usando exchange moderno com routing key
                if (_messagePublisher is RabbitMQPublisher publisher)
                {
                    await publisher.PublishToExchangeAsync(
                        ExchangeSettings.STOCK_EXCHANGE,
                        ExchangeSettings.STOCK_REDUCTION_ROUTING_KEY,
                        stockReducedEvent);
                }
                else
                {
                    // Fallback para compatibilidade
                    await _messagePublisher.PublishAsync(stockReducedEvent, ExchangeSettings.STOCK_REDUCTION_QUEUE);
                }
                
                _logger.LogInformation(
                    "Notificação de redução de estoque enviada via exchange {Exchange} com routing key {RoutingKey}. ProductId: {ProductId}, Quantity: {Quantity}, OrderId: {OrderId}",
                    ExchangeSettings.STOCK_EXCHANGE, ExchangeSettings.STOCK_REDUCTION_ROUTING_KEY,
                    productId, quantity, orderId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, 
                    "Erro ao enviar notificação de redução de estoque. ProductId: {ProductId}, Quantity: {Quantity}, OrderId: {OrderId}",
                    productId, quantity, orderId);
                throw;
            }
        }
    }
}
