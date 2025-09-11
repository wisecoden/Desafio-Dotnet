namespace AvanadeAwesomeShop.Service.Orders.Domain.Entities
{
    public class OrderItem
    {
        public Guid Id { get; private set; }
        public Guid ProductId { get; private set; }
        public int Quantity { get; private set; }
        public decimal Price { get; private set; }
        
        // Construtor parameterless para EF
        private OrderItem()
        {
            Id = Guid.NewGuid();
        }
        public OrderItem(Guid productId, int quantity, decimal price) : this()
        {
            ProductId = productId;
            Quantity = quantity;
            Price = price;
        }

        public void UpdateQuantity(int newQuantity)
        {
            if (newQuantity <= 0)
                throw new ArgumentException("Quantidade deve ser maior que zero", nameof(newQuantity));
            
            Quantity = newQuantity;
        }

        public void UpdatePrice(decimal newPrice)
        {
            if (newPrice <= 0)
                throw new ArgumentException("Preço unitário deve ser maior que zero", nameof(newPrice));
            
            Price = newPrice;
        }
    }
}
