using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AvanadeAwesomeShop.Service.Orders.Domain.Entities;
using AvanadeAwesomeShop.Service.Orders.Domain.Enums;

namespace AvanadeAwesomeShop.Service.Orders.Infrastructure.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasKey(o => o.Id);

            builder.Property(o => o.Id)
                .ValueGeneratedNever();

            builder.Property(o => o.CustomerId)
                .IsRequired();

            builder.Property(o => o.Status)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(o => o.TotalPrice)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(o => o.CreatedAt)
                .IsRequired();

            builder.Property(o => o.UpdatedAt)
                .IsRequired();

            // Configuração do relacionamento com OrderItems
            builder.HasMany(o => o.Items)
                .WithOne()
                .HasForeignKey("OrderId")
                .OnDelete(DeleteBehavior.Cascade);

            // Ignorar a propriedade privada _items para evitar conflito
            builder.Ignore("_items");

            // Ignorar propriedades do AggregateRoot que não devem ser persistidas
            builder.Ignore(o => o.DomainEvents);

            builder.ToTable("Orders");

        }
    }
}
