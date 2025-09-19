using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AvanadeAwesomeShop.Service.Stock.Infrastructure.Data;

public class StockDbContextFactory : IDesignTimeDbContextFactory<StockDbContext>
{
    public StockDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<StockDbContext>();
        optionsBuilder.UseSqlServer("Server=tcp:localhost,14330;Database=AvanadeAwesomeShop-Stock;User Id=sa;Password=AvanadeAwesome123!;TrustServerCertificate=true;MultipleActiveResultSets=true");

        return new StockDbContext(optionsBuilder.Options);
    }
}
