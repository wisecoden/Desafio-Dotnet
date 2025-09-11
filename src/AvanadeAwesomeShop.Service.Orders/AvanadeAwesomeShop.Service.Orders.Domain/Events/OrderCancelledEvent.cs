namespace AvanadeAwesomeShop.Service.Orders.Domain.Events
{
    public class OrderCancelledEvent : DomainEvent
    {
        public Guid CustomerId { get; private set; }
        public string Reason { get; private set; }
        public decimal TotalAmount { get; private set; }

        public OrderCancelledEvent(Guid orderId, Guid customerId, string reason, decimal totalAmount) 
            : base(orderId)
        {
            CustomerId = customerId;
            Reason = reason;
            TotalAmount = totalAmount;
        }
    }
}
