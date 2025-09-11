namespace AvanadeAwesomeShop.Service.Orders.Domain.Events
{
    public interface IDomainEvent
    {
        Guid AggregateId { get; }
        DateTime OccurredOn { get; }
    }
}
