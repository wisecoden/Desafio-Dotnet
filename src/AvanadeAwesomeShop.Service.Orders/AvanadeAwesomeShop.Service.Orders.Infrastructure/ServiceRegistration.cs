using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AvanadeAwesomeShop.Service.Orders.Domain.Repositories;
using AvanadeAwesomeShop.Service.Orders.Infrastructure.Data;
using AvanadeAwesomeShop.Service.Orders.Infrastructure.Repositories;
using AvanadeAwesomeShop.Service.Orders.Infrastructure.MessageBus;
using AvanadeAwesomeShop.Service.Orders.Infrastructure.MessageBus.Services;
using AvanadeAwesomeShop.Service.Orders.Infrastructure.MessageBus.Configuration;
using AvanadeAwesomeShop.Service.Orders.Application.Services;
using AvanadeAwesomeShop.Service.Orders.Infrastructure.Services;
using RabbitMQ.Client;

namespace AvanadeAwesomeShop.Service.Orders.Infrastructure
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Database
            services.AddDbContext<OrdersDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // Repositories
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();

            // Integração com Stock
            services.AddHttpClient<IStockService, StockService>(client =>
            {
                client.BaseAddress = new Uri("http://localhost:5063");
                client.Timeout = TimeSpan.FromSeconds(5); // Timeout de 5 segundos
            });
            services.AddScoped<IStockService, StockService>();

            // RabbitMQ Configuration
            var rabbitMQSettings = new RabbitMQSettings();
            configuration.GetSection("RabbitMQ").Bind(rabbitMQSettings);
            services.AddSingleton(rabbitMQSettings);
            
            // RabbitMQ Connection (Obrigatório)
            services.AddSingleton<IConnection>(serviceProvider =>
            {
                var settings = serviceProvider.GetRequiredService<RabbitMQSettings>();
                
                Console.WriteLine($"🔧 Conectando RabbitMQ: {settings.HostName}:{settings.Port}");
                
                var factory = new ConnectionFactory()
                {
                    HostName = settings.HostName,
                    Port = settings.Port,
                    UserName = settings.UserName,
                    Password = settings.Password,
                    VirtualHost = settings.VirtualHost
                };

                var connection = factory.CreateConnectionAsync().Result;
                Console.WriteLine("✅ RabbitMQ conectado com sucesso!");
                return connection;
            });

            // Messaging Services
            services.AddSingleton<IMessagePublisher, RabbitMQPublisher>();
            services.AddSingleton<IStockNotificationService, StockNotificationService>();
            services.AddSingleton<IRabbitMQInitializer, RabbitMQInitializer>();

            return services;
        }
    }
}
