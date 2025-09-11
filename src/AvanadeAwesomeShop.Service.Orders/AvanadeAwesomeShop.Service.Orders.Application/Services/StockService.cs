using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;

namespace AvanadeAwesomeShop.Service.Orders.Application.Services
{
  public interface
  IStockService
  {
    Task<bool> HasStockAsync(Guid productId, int quantity);
  }

  public class StockService : IStockService
  {
    private readonly HttpClient _httpClient;
    public StockService(HttpClient httpClient)
    {
      _httpClient = httpClient;
    }

    public async Task<bool> HasStockAsync(Guid productId, int quantity)
    {
      try
      {
        var response = await _httpClient.GetAsync($"/api/products/{productId}");
        if (!response.IsSuccessStatusCode)
          return false; // Considera como sem estoque se API falhar

        var json = await response.Content.ReadAsStringAsync();
        var product = JsonSerializer.Deserialize<ProductStockDto>(json, new JsonSerializerOptions
        {
          PropertyNameCaseInsensitive = true
        });

        return product != null && product.Quantity >= quantity;
      }
      catch (HttpRequestException)
      {

        return false;
      }
      catch (TaskCanceledException)
      {
        // Timeout
        return false;
      }
    }

  }

    public class ProductStockDto
  {
    public Guid Id { get; set; }
    public int Quantity { get; set; }
  }
}
