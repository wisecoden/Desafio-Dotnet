namespace AvanadeAwesomeShop.Service.Stock.Domain.Entities
{
    public class Product : AggregateRoot
    {
        public string Name { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;
        public string Category { get; private set; } = string.Empty;
        public decimal Price { get; private set; }
        public int Quantity { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }

        public Product()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public Product(string name, string description, string category, decimal price, int initialStock) : this()
        {
            Name = name;
            Description = description;
            Category = category;
            Price = price;
            Quantity = initialStock;
        }

        public bool HasStock(int quantity) => Quantity >= quantity;
        
        public void UpdateStock(int quantity)
        {
            Quantity = quantity;
            UpdateAt();
        }

        public void UpdateAt()
        {
            UpdatedAt = DateTime.UtcNow;
        }
   
        public void ReduceStock(int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantidade deve ser maior que zero");

            if (!HasStock(quantity))
                throw new InvalidOperationException("Estoque insuficiente");

            Quantity -= quantity;
            UpdatedAt = DateTime.UtcNow;
        }

        public void AddStock(int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantidade deve ser maior que zero");

            Quantity += quantity;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
