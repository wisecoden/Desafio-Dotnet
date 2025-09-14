namespace AvanadeAwesomeShop.Service.Orders.Domain.Events
{
    public class OrderConfirmedEvent : DomainEvent
    {
        public Guid CustomerId { get; private set; }
        public decimal TotalPrice { get; private set; }
        public List<OrderItemData> Items { get; private set; }

        public OrderConfirmedEvent(Guid orderId, Guid customerId, decimal totalprice, List<OrderItemData> items) 
            : base(orderId)
        {
            CustomerId = customerId;
            TotalPrice = totalprice;
            Items = items;
        }
    }

    public class OrderItemData
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        
        public OrderItemData(Guid productId, int quantity, decimal price)
        {
            ProductId = productId;
            Quantity = quantity;
            Price = price;
        }
    }
}
