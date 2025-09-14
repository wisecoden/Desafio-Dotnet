using Microsoft.EntityFrameworkCore;
using MediatR;
using AvanadeAwesomeShop.Service.Orders.Domain.Entities;
using AvanadeAwesomeShop.Service.Orders.Infrastructure.Configurations;
using AvanadeAwesomeShop.Service.Orders.Application.Events;

namespace AvanadeAwesomeShop.Service.Orders.Infrastructure.Data
{
    public class OrdersDbContext : DbContext
    {
        private readonly IMediator? _mediator;

        public OrdersDbContext(DbContextOptions<OrdersDbContext> options, IMediator? mediator = null) 
            : base(options)
        {
            _mediator = mediator;
        }

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Customer> Customers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Aplicar todas as configurações
            modelBuilder.ApplyConfiguration(new OrderConfiguration());
            modelBuilder.ApplyConfiguration(new OrderItemConfiguration());
            modelBuilder.ApplyConfiguration(new CustomerConfiguration());

            // Configurações globais
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(decimal) || property.ClrType == typeof(decimal?))
                    {
                        property.SetColumnType("decimal(18,2)");
                    }
                }
            }
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // Coletar domain events antes de salvar
            var domainEntities = ChangeTracker.Entries<AggregateRoot>()
                .Where(e => e.Entity.DomainEvents.Any())
                .ToList();

            var domainEvents = domainEntities
                .SelectMany(e => e.Entity.DomainEvents)
                .ToList();

            // Limpar domain events das entidades
            domainEntities.ForEach(e => e.Entity.ClearDomainEvents());

            // Salvar mudanças no banco primeiro
            var result = await base.SaveChangesAsync(cancellationToken);

            // Publicar domain events via MediatR usando o adapter
            if (_mediator != null)
            {
                foreach (var domainEvent in domainEvents)
                {
                    var notificationType = typeof(DomainEventNotification<>).MakeGenericType(domainEvent.GetType());
                    var notification = Activator.CreateInstance(notificationType, domainEvent);
                    
                    if (notification != null)
                    {
                        await _mediator.Publish(notification, cancellationToken);
                    }
                }
            }

            return result;
        }
    }
}
