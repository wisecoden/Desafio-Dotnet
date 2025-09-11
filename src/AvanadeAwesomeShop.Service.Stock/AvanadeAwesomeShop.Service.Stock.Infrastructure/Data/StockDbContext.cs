using Microsoft.EntityFrameworkCore;
using AvanadeAwesomeShop.Service.Stock.Domain.Entities;

namespace AvanadeAwesomeShop.Service.Stock.Infrastructure.Data;

public class StockDbContext : DbContext
{
    public StockDbContext(DbContextOptions<StockDbContext> options) : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Product Configuration
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.Category)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.Description)
                .HasMaxLength(500);

            entity.Property(e => e.Price)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            entity.Property(e => e.Quantity)
                .IsRequired();

            entity.Property(e => e.CreatedAt)
                .IsRequired();

            entity.Property(e => e.UpdatedAt)
                .IsRequired();

            entity.HasIndex(e => e.Category);
        });
    }
}
