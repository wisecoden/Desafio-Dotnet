namespace AvanadeAwesomeShop.Service.Orders.Domain.Events
{
    public class OrderConfirmedEvent : DomainEvent
    {
        public Guid CustomerId { get; private set; }
        public decimal TotalAmount { get; private set; }
        public List<OrderItemData> Items { get; private set; }

        public OrderConfirmedEvent(Guid orderId, Guid customerId, decimal totalAmount, List<OrderItemData> items) 
            : base(orderId)
        {
            CustomerId = customerId;
            TotalAmount = totalAmount;
            Items = items;
        }
    }

    public class OrderItemData
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        
        public OrderItemData(Guid productId, int quantity, decimal unitPrice)
        {
            ProductId = productId;
            Quantity = quantity;
            UnitPrice = unitPrice;
        }
    }
}
