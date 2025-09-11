using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AvanadeAwesomeShop.Service.Stock.Domain.Repositories;
using AvanadeAwesomeShop.Service.Stock.Infrastructure.Data;
using AvanadeAwesomeShop.Service.Stock.Infrastructure.Repositories;
using AvanadeAwesomeShop.Service.Stock.Infrastructure.MessageBus.Configuration;
using AvanadeAwesomeShop.Service.Stock.Infrastructure.MessageBus.Services;
using RabbitMQ.Client;

namespace AvanadeAwesomeShop.Service.Stock.Infrastructure;

public static class ServiceRegistration
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Database
        services.AddDbContext<StockDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        // Repositories
        services.AddScoped<IProductRepository, ProductRepository>();

        // RabbitMQ Configuration
        var rabbitMQSettings = new RabbitMQSettings();
        configuration.GetSection("RabbitMQ").Bind(rabbitMQSettings);
        services.AddSingleton(rabbitMQSettings);
        
        // RabbitMQ Connection (Obrigatório)
        services.AddSingleton<IConnection>(serviceProvider =>
        {
            var settings = serviceProvider.GetRequiredService<RabbitMQSettings>();
            
            Console.WriteLine($"🔧 Conectando RabbitMQ Stock: {settings.HostName}:{settings.Port}");
            
            var factory = new ConnectionFactory()
            {
                HostName = settings.HostName,
                Port = settings.Port,
                UserName = settings.UserName,
                Password = settings.Password,
                VirtualHost = settings.VirtualHost
            };

            var connection = factory.CreateConnectionAsync().Result;
            Console.WriteLine("✅ RabbitMQ Stock conectado com sucesso!");
            return connection;
        });

        // RabbitMQ Initializer
        services.AddSingleton<IRabbitMQInitializer, RabbitMQInitializer>();

        // Background Services
        services.AddHostedService<StockReductionConsumer>();

        return services;
    }
}
