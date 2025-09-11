namespace AvanadeAwesomeShop.Service.Stock.Infrastructure.MessageBus.Configuration;

public class ExchangeSettings
{
    public const string STOCK_EXCHANGE = "stock.exchange";
    public const string ORDERS_EXCHANGE = "orders.exchange";
    public const string NOTIFICATIONS_EXCHANGE = "notifications.exchange";
    
    // Routing Keys
    public const string STOCK_REDUCTION_ROUTING_KEY = "stock.reduction";
    public const string STOCK_UPDATED_ROUTING_KEY = "stock.updated";
    public const string PRODUCT_CREATED_ROUTING_KEY = "product.created";
    
    // Queue Names
    public const string STOCK_REDUCTION_QUEUE = "stock-reduction-queue";
    public const string STOCK_UPDATE_QUEUE = "stock-update-queue";
}
