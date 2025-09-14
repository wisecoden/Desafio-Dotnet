namespace AvanadeAwesomeShop.Service.Orders.Domain.Events
{
    public class OrderCreatedEvent : DomainEvent
    {
        public Guid CustomerId { get; private set; }
        public decimal TotalPrice { get; private set; }
        public int ItemsCount { get; private set; }

        public OrderCreatedEvent(Guid orderId, Guid customerId, decimal totalprice, int itemsCount) 
            : base(orderId)
        {
            CustomerId = customerId;
            TotalPrice = totalprice;
            ItemsCount = itemsCount;
        }
    }
}
