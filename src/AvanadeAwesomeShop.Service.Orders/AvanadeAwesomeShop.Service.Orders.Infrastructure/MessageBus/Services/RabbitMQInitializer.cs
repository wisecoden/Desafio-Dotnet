using RabbitMQ.Client;
using AvanadeAwesomeShop.Service.Orders.Infrastructure.MessageBus.Configuration;

namespace AvanadeAwesomeShop.Service.Orders.Infrastructure.MessageBus.Services;

public interface IRabbitMQInitializer
{
    Task InitializeAsync();
}

public class RabbitMQInitializer : IRabbitMQInitializer
{
    private readonly IConnection _connection;

    public RabbitMQInitializer(IConnection connection)
    {
        _connection = connection;
    }

    public async Task InitializeAsync()
    {
        using var channel = await _connection.CreateChannelAsync();

        // Declarar Exchanges
        await channel.ExchangeDeclareAsync(
            exchange: ExchangeSettings.STOCK_EXCHANGE,
            type: ExchangeType.Topic,
            durable: true,
            autoDelete: false);

        await channel.ExchangeDeclareAsync(
            exchange: ExchangeSettings.ORDERS_EXCHANGE,
            type: ExchangeType.Topic,
            durable: true,
            autoDelete: false);

        await channel.ExchangeDeclareAsync(
            exchange: ExchangeSettings.NOTIFICATIONS_EXCHANGE,
            type: ExchangeType.Topic,
            durable: true,
            autoDelete: false);

        // Declarar Queues
        await channel.QueueDeclareAsync(
            queue: ExchangeSettings.STOCK_REDUCTION_QUEUE,
            durable: true,
            exclusive: false,
            autoDelete: false);

        await channel.QueueDeclareAsync(
            queue: ExchangeSettings.ORDER_NOTIFICATION_QUEUE,
            durable: true,
            exclusive: false,
            autoDelete: false);

        // Bind Queues to Exchanges
        await channel.QueueBindAsync(
            queue: ExchangeSettings.STOCK_REDUCTION_QUEUE,
            exchange: ExchangeSettings.STOCK_EXCHANGE,
            routingKey: ExchangeSettings.STOCK_REDUCTION_ROUTING_KEY);

        await channel.QueueBindAsync(
            queue: ExchangeSettings.ORDER_NOTIFICATION_QUEUE,
            exchange: ExchangeSettings.NOTIFICATIONS_EXCHANGE,
            routingKey: ExchangeSettings.ORDER_CONFIRMED_ROUTING_KEY);

        Console.WriteLine("✅ RabbitMQ Exchanges e Queues inicializados com sucesso!");
    }
}
