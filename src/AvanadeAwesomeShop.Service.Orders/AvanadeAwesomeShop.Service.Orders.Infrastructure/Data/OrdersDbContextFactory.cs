using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AvanadeAwesomeShop.Service.Orders.Infrastructure.Data
{
    public class OrdersDbContextFactory : IDesignTimeDbContextFactory<OrdersDbContext>
    {
        public OrdersDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<OrdersDbContext>();
            
            optionsBuilder.UseSqlServer("Server=localhost,14330;Database=AvanadeAwesomeShop-Orders;User Id=sa;Password=AvanadeAwesome123!;TrustServerCertificate=true;MultipleActiveResultSets=true");

            return new OrdersDbContext(optionsBuilder.Options);
        }
    }
}
