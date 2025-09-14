namespace AvanadeAwesomeShop.Service.Orders.Application.Services
{
    public interface IStockService
    {
        Task<bool> HasStockAsync(Guid productId, int quantity);
        Task<ProductStockDto> GetProductStockAsync(Guid productId);
    }

    public class ProductStockDto
    {
        public Guid Id { get; set; } 
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; } 
        
        // Propriedades calculadas para compatibilidade
        public Guid ProductId => Id;
        public int StockQuantity => Quantity;
        public bool IsAvailable => Quantity > 0;
    }
}