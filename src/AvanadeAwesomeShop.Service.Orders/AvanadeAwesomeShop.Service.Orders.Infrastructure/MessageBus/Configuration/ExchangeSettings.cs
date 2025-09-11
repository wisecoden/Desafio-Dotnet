namespace AvanadeAwesomeShop.Service.Orders.Infrastructure.MessageBus.Configuration;

public class ExchangeSettings
{
    public const string STOCK_EXCHANGE = "stock.exchange";
    public const string ORDERS_EXCHANGE = "orders.exchange";
    public const string NOTIFICATIONS_EXCHANGE = "notifications.exchange";
    
    // Routing Keys
    public const string STOCK_REDUCTION_ROUTING_KEY = "stock.reduction";
    public const string ORDER_CONFIRMED_ROUTING_KEY = "order.confirmed";
    public const string ORDER_CANCELLED_ROUTING_KEY = "order.cancelled";
    
    // Queue Names
    public const string STOCK_REDUCTION_QUEUE = "stock-reduction-queue";
    public const string ORDER_NOTIFICATION_QUEUE = "order-notification-queue";
}
