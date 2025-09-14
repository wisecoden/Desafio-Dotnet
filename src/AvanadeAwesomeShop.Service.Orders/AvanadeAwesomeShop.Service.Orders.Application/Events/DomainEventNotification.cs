using MediatR;
using AvanadeAwesomeShop.Service.Orders.Domain.Events;

namespace AvanadeAwesomeShop.Service.Orders.Application.Events
{
    // Adapter pattern para conectar Domain Events com MediatR
    public class DomainEventNotification<TDomainEvent> : INotification
        where TDomainEvent : IDomainEvent
    {
        public TDomainEvent DomainEvent { get; }

        public DomainEventNotification(TDomainEvent domainEvent)
        {
            DomainEvent = domainEvent;
        }
    }
}