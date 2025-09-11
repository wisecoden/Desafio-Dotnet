namespace AvanadeAwesomeShop.Service.Stock.Infrastructure.MessageBus.Events
{
    public class StockReducedEvent
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public Guid OrderId { get; set; }
        public DateTime OccurredAt { get; set; }

        public StockReducedEvent(Guid productId, int quantity, Guid orderId)
        {
            ProductId = productId;
            Quantity = quantity;
            OrderId = orderId;
            OccurredAt = DateTime.UtcNow;
        }

        // Construtor sem parâmetros para deserialização
        public StockReducedEvent()
        {
            OccurredAt = DateTime.UtcNow;
        }
    }
}
