using AvanadeAwesomeShop.Service.Stock.Domain.Entities;

namespace AvanadeAwesomeShop.Service.Stock.Domain.Repositories;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(Guid id);
    Task<IEnumerable<Product>> GetAllAsync();
    Task<IEnumerable<Product>> GetByCategoryAsync(string category);
    Task AddAsync(Product product);
    Task UpdateAsync(Product product);
}
