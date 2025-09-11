using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AvanadeAwesomeShop.Service.Orders.Infrastructure.Data
{
    public class OrdersDbContextFactory : IDesignTimeDbContextFactory<OrdersDbContext>
    {
        public OrdersDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<OrdersDbContext>();
            

            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=AvanadeAwesomeShop-Orders;Trusted_Connection=true;MultipleActiveResultSets=true");

            return new OrdersDbContext(optionsBuilder.Options);
        }
    }
}
