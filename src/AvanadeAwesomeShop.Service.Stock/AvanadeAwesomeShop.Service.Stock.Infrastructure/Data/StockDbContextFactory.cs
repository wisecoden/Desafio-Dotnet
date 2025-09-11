using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AvanadeAwesomeShop.Service.Stock.Infrastructure.Data;

public class StockDbContextFactory : IDesignTimeDbContextFactory<StockDbContext>
{
    public StockDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<StockDbContext>();
        optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=AvanadeAwesomeShop-Stock;Trusted_Connection=true;MultipleActiveResultSets=true");

        return new StockDbContext(optionsBuilder.Options);
    }
}
