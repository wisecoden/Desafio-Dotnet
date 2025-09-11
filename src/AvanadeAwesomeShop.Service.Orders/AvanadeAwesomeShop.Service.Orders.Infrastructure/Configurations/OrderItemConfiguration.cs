using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AvanadeAwesomeShop.Service.Orders.Domain.Entities;

namespace AvanadeAwesomeShop.Service.Orders.Infrastructure.Configurations
{
    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.HasKey(oi => oi.Id);

            builder.Property(oi => oi.Id)
                .ValueGeneratedNever();

            builder.Property(oi => oi.ProductId)
                .IsRequired();

            builder.Property(oi => oi.Quantity)
                .IsRequired();

            builder.Property(oi => oi.Price)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            // Adicionar coluna OrderId como chave estrangeira
            builder.Property<Guid>("OrderId")
                .IsRequired();

            builder.HasIndex("OrderId");

            builder.ToTable("OrderItems");
        }
    }
}
