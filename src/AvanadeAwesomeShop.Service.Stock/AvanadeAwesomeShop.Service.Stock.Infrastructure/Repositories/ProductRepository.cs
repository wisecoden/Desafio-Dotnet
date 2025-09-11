using Microsoft.EntityFrameworkCore;
using AvanadeAwesomeShop.Service.Stock.Domain.Entities;
using AvanadeAwesomeShop.Service.Stock.Domain.Repositories;
using AvanadeAwesomeShop.Service.Stock.Infrastructure.Data;

namespace AvanadeAwesomeShop.Service.Stock.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly StockDbContext _context;

    public ProductRepository(StockDbContext context)
    {
        _context = context;
    }

    public async Task<Product?> GetByIdAsync(Guid id)
    {
        return await _context.Products.FindAsync(id);
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        return await _context.Products.ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetByCategoryAsync(string category)
    {
        return await _context.Products
            .Where(p => p.Category.ToLower() == category.ToLower())
            .ToListAsync();
    }

    public async Task AddAsync(Product product)
    {
        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Product product)
    {
        product.UpdateAt();
        _context.Products.Update(product);
        await _context.SaveChangesAsync();
    }
}
