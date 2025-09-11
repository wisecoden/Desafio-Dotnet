namespace AvanadeAwesomeShop.Service.Orders.Domain.Events
{
    public class OrderCreatedEvent : DomainEvent
    {
        public Guid CustomerId { get; private set; }
        public decimal TotalAmount { get; private set; }
        public int ItemsCount { get; private set; }

        public OrderCreatedEvent(Guid orderId, Guid customerId, decimal totalAmount, int itemsCount) 
            : base(orderId)
        {
            CustomerId = customerId;
            TotalAmount = totalAmount;
            ItemsCount = itemsCount;
        }
    }
}
