using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using AvanadeAwesomeShop.Service.Stock.Infrastructure.MessageBus.Events;
using AvanadeAwesomeShop.Service.Stock.Infrastructure.MessageBus.Configuration;
using AvanadeAwesomeShop.Service.Stock.Domain.Repositories;

namespace AvanadeAwesomeShop.Service.Stock.Infrastructure.MessageBus.Services
{
    public class StockReductionConsumer : BackgroundService
    {
        private readonly ILogger<StockReductionConsumer> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly RabbitMQSettings _rabbitMQSettings;
        private readonly IConfiguration _configuration;
        private IConnection? _connection;
        private IChannel? _channel;
        private const string STOCK_QUEUE = "stock-reduction-queue";

        public StockReductionConsumer(
            ILogger<StockReductionConsumer> logger,
            IServiceProvider serviceProvider,
            RabbitMQSettings rabbitMQSettings,
            IConfiguration configuration)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _rabbitMQSettings = rabbitMQSettings;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Verifica se RabbitMQ está habilitado
            var rabbitMQEnabled = _configuration.GetValue<bool>("RabbitMQ:Enabled", true);
            
            if (!rabbitMQEnabled)
            {
                _logger.LogInformation("RabbitMQ está desabilitado. Consumer não será iniciado.");
                return;
            }

            try
            {
                await InitializeRabbitMQ();

                var consumer = new AsyncEventingBasicConsumer(_channel!);
                consumer.ReceivedAsync += async (model, ea) =>
                {
                    try
                    {
                        var body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);
                        
                        _logger.LogInformation("Mensagem recebida: {Message}", message);

                        var stockReducedEvent = JsonSerializer.Deserialize<StockReducedEvent>(message);
                        
                        if (stockReducedEvent != null)
                        {
                            await ProcessStockReduction(stockReducedEvent);
                            await _channel!.BasicAckAsync(ea.DeliveryTag, false);
                            _logger.LogInformation("Estoque reduzido com sucesso para o produto {ProductId}", stockReducedEvent.ProductId);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Erro ao processar mensagem de redução de estoque");
                        await _channel!.BasicNackAsync(ea.DeliveryTag, false, true);
                    }
                };

                await _channel!.BasicConsumeAsync(
                    queue: STOCK_QUEUE,
                    autoAck: false,
                    consumer: consumer);

                _logger.LogInformation("Consumidor de redução de estoque iniciado. Aguardando mensagens...");

                while (!stoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(1000, stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao inicializar o consumidor RabbitMQ. Serviço continuará sem mensageria.");
                // Em produção, você pode escolher falhar ou continuar sem RabbitMQ
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken); // Aguarda antes de encerrar
            }
        }

        private async Task InitializeRabbitMQ()
        {
            var factory = new ConnectionFactory()
            {
                HostName = _rabbitMQSettings.HostName,
                Port = _rabbitMQSettings.Port,
                UserName = _rabbitMQSettings.UserName,
                Password = _rabbitMQSettings.Password,
                VirtualHost = _rabbitMQSettings.VirtualHost
            };

            try
            {
                _connection = await factory.CreateConnectionAsync();
                _channel = await _connection.CreateChannelAsync();

                // Declara a fila se não existir
                await _channel.QueueDeclareAsync(
                    queue: STOCK_QUEUE,
                    durable: true,
                    exclusive: false,
                    autoDelete: false);

                _logger.LogInformation("Conexão RabbitMQ estabelecida e fila {Queue} declarada", STOCK_QUEUE);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Falha ao conectar com RabbitMQ");
                throw; // Re-lança para ser capturada no ExecuteAsync
            }
        }

        private async Task ProcessStockReduction(StockReducedEvent stockReducedEvent)
        {
            using var scope = _serviceProvider.CreateScope();
            var productRepository = scope.ServiceProvider.GetRequiredService<IProductRepository>();

            var product = await productRepository.GetByIdAsync(stockReducedEvent.ProductId);
            
            if (product == null)
            {
                _logger.LogWarning("Produto {ProductId} não encontrado para redução de estoque", stockReducedEvent.ProductId);
                return;
            }

            try
            {
                product.ReduceStock(stockReducedEvent.Quantity);
                await productRepository.UpdateAsync(product);
                
                _logger.LogInformation(
                    "Estoque do produto {ProductId} reduzido em {Quantity} unidades. Pedido: {OrderId}",
                    stockReducedEvent.ProductId, stockReducedEvent.Quantity, stockReducedEvent.OrderId);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex,
                    "Não foi possível reduzir o estoque do produto {ProductId}. Quantidade solicitada: {Quantity}",
                    stockReducedEvent.ProductId, stockReducedEvent.Quantity);
                throw;
            }
        }

        public override void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
            base.Dispose();
        }
    }
}
