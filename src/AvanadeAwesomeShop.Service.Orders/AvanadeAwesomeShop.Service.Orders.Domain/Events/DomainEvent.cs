namespace AvanadeAwesomeShop.Service.Orders.Domain.Events
{
    public abstract class DomainEvent : IDomainEvent
    {
        public Guid AggregateId { get; private set; }
        public DateTime OccurredOn { get; private set; }

        protected DomainEvent(Guid aggregateId)
        {
            AggregateId = aggregateId;
            OccurredOn = DateTime.UtcNow;
        }
    }
}
