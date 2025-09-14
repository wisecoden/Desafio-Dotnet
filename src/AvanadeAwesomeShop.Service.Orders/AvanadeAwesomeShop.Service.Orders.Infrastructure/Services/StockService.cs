using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using AvanadeAwesomeShop.Service.Orders.Application.Services;

namespace AvanadeAwesomeShop.Service.Orders.Infrastructure.Services
{
    public class StockService : IStockService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<StockService> _logger;
        private readonly string _stockServiceBaseUrl;

        public StockService(HttpClient httpClient, IConfiguration configuration, ILogger<StockService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _stockServiceBaseUrl = configuration["Services:Stock:BaseUrl"] ?? "https://localhost:5001";
        }

        public async Task<bool> HasStockAsync(Guid productId, int quantity)
        {
            try
            {
                _logger.LogInformation("Checking stock for ProductId: {ProductId}, Quantity: {Quantity}", productId, quantity);
                var productStock = await GetProductStockAsync(productId);
                var result = productStock.IsAvailable && productStock.StockQuantity >= quantity;
                _logger.LogInformation("Stock check result - Available: {IsAvailable}, Stock: {StockQuantity}, Required: {Quantity}, Result: {Result}", 
                    productStock.IsAvailable, productStock.StockQuantity, quantity, result);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking stock for product {ProductId}", productId);
                return false; // Em caso de erro, considera que não há estoque
            }
        }

        public async Task<ProductStockDto> GetProductStockAsync(Guid productId)
        {
            try
            {
                _logger.LogInformation("Calling Stock API for ProductId: {ProductId} at {BaseUrl}", productId, _stockServiceBaseUrl);
                var response = await _httpClient.GetAsync($"{_stockServiceBaseUrl}/api/products/{productId}");
                
                _logger.LogInformation("Stock API response: StatusCode {StatusCode}", response.StatusCode);
                
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Stock service returned status {StatusCode} for product {ProductId}", 
                        response.StatusCode, productId);
                    
                    return new ProductStockDto 
                    { 
                        Id = productId, 
                        Quantity = 0 
                    };
                }

                var jsonContent = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("Stock API response content: {JsonContent}", jsonContent);
                
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var stockDto = JsonSerializer.Deserialize<ProductStockDto>(jsonContent, options);

                if (stockDto != null)
                {
                    _logger.LogInformation("Deserialized ProductStock - Id: {Id}, ProductId: {ProductId}, Available: {IsAvailable}, Stock: {StockQuantity}, Quantity: {Quantity}", 
                        stockDto.Id, stockDto.ProductId, stockDto.IsAvailable, stockDto.StockQuantity, stockDto.Quantity);
                }

                return stockDto ?? new ProductStockDto 
                { 
                    Id = productId, 
                    Quantity = 0 
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting stock for product {ProductId}", productId);
                return new ProductStockDto 
                { 
                    Id = productId, 
                    Quantity = 0 
                };
            }
        }
    }
}