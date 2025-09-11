namespace AvanadeAwesomeShop.Service.Orders.Infrastructure.MessageBus
{
    public interface IMessagePublisher
    {
        Task PublishAsync<T>(T message, string exchangeName, string routingKey) where T : class;
        Task PublishAsync<T>(T message, string queueName) where T : class;
    }
}
