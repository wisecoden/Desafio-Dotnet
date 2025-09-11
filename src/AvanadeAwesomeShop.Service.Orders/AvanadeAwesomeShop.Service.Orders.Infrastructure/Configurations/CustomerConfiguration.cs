using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AvanadeAwesomeShop.Service.Orders.Domain.Entities;

namespace AvanadeAwesomeShop.Service.Orders.Infrastructure.Configurations
{
    public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Id)
                .ValueGeneratedNever();

            builder.Property(c => c.FullName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(c => c.Email)
                .IsRequired()
                .HasMaxLength(255);

            builder.HasIndex(c => c.Email)
                .IsUnique();

            builder.Property(c => c.CreatedAt)
                .IsRequired();

            // Ignorar propriedades do AggregateRoot que não devem ser persistidas
            builder.Ignore(c => c.DomainEvents);

            builder.ToTable("Customers");
        
        }
    }
}
