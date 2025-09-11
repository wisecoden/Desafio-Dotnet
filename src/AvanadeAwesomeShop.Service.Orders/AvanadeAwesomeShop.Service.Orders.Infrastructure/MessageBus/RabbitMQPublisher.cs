using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace AvanadeAwesomeShop.Service.Orders.Infrastructure.MessageBus
{
    public class RabbitMQPublisher : IMessagePublisher, IDisposable
    {
        private readonly IConnection? _connection;
        private readonly IChannel? _channel;
        private readonly ILogger<RabbitMQPublisher> _logger;
        private readonly bool _rabbitMQEnabled;

        public RabbitMQPublisher(IConnection? connection, ILogger<RabbitMQPublisher> logger, IConfiguration configuration)
        {
            _connection = connection;
            _logger = logger;
            _rabbitMQEnabled = configuration.GetValue<bool>("RabbitMQ:Enabled", true);
            
            _logger.LogInformation("🔌 Inicializando RabbitMQPublisher. Habilitado: {Enabled}, Conexão fornecida: {ConnectionProvided}", 
                _rabbitMQEnabled, connection != null);
            
            if (_connection != null && _rabbitMQEnabled)
            {
                try
                {
                    _logger.LogInformation("🔄 Criando canal RabbitMQ...");
                    _channel = _connection.CreateChannelAsync().Result;
                    _logger.LogInformation("✅ Canal RabbitMQ criado com sucesso!");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ Não foi possível criar canal RabbitMQ. Mensagens não serão enviadas.");
                }
            }
            else
            {
                _logger.LogWarning("⚠️ RabbitMQ não será usado. Habilitado: {Enabled}, Conexão: {ConnectionAvailable}", 
                    _rabbitMQEnabled, _connection != null);
            }
        }

        public async Task PublishAsync<T>(T message, string exchangeName, string routingKey) where T : class
        {
            if (!_rabbitMQEnabled)
            {
                _logger.LogInformation("RabbitMQ desabilitado. Mensagem não enviada: {Message}", JsonSerializer.Serialize(message));
                return;
            }

            // Verifica e recria o canal se necessário
            if (_channel == null || !_channel.IsOpen)
            {
                if (_connection != null && _connection.IsOpen)
                {
                    try
                    {
                        _logger.LogInformation("🔄 Canal RabbitMQ fechado ou nulo. Tentando recriar...");
                        var channel = _connection.CreateChannelAsync().Result;
                        // Como _channel é readonly, use reflection para setar (ou refatore para não readonly)
                        typeof(RabbitMQPublisher)
                            .GetField("_channel", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                            ?.SetValue(this, channel);
                        _logger.LogInformation("✅ Canal RabbitMQ recriado com sucesso!");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "❌ Falha ao recriar canal RabbitMQ.");
                        return;
                    }
                }
                else
                {
                    _logger.LogWarning("❌ Conexão RabbitMQ não disponível. Mensagem não enviada.");
                    return;
                }
            }

            try
            {
                var json = JsonSerializer.Serialize(message);
                var body = Encoding.UTF8.GetBytes(json);

                if (_channel != null) // ✅ Verificação explícita
                {
                    await _channel.BasicPublishAsync(
                        exchange: exchangeName,
                        routingKey: routingKey,
                        body: body);
                    
                    _logger.LogInformation("Mensagem enviada para exchange {Exchange} com routing key {RoutingKey}", exchangeName, routingKey);
                }
                else
                {
                    _logger.LogWarning("Canal RabbitMQ não disponível durante publicação");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao publicar mensagem no RabbitMQ");
            }
        }

        public async Task PublishAsync<T>(T message, string queueName) where T : class
        {
            _logger.LogInformation("🔄 Iniciando PublishAsync para fila {Queue}. RabbitMQ habilitado: {Enabled}, Canal disponível: {ChannelAvailable}", 
                queueName, _rabbitMQEnabled, _channel != null);

            if (!_rabbitMQEnabled)
            {
                _logger.LogInformation("RabbitMQ desabilitado. Mensagem não enviada para fila {Queue}: {Message}", queueName, JsonSerializer.Serialize(message));
                return;
            }

            if (_channel == null)
            {
                _logger.LogWarning("❌ Canal RabbitMQ não disponível. Mensagem não enviada para fila {Queue}: {Message}", queueName, JsonSerializer.Serialize(message));
                return;
            }

            try
            {
                _logger.LogInformation("📋 Declarando fila {Queue}...", queueName);
                
                // Declara a fila se não existir
                await _channel.QueueDeclareAsync(
                    queue: queueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false);

                var json = JsonSerializer.Serialize(message);
                var body = Encoding.UTF8.GetBytes(json);

                _logger.LogInformation("📨 Enviando mensagem para fila {Queue}: {Message}", queueName, json);

                await _channel.BasicPublishAsync(
                    exchange: string.Empty,
                    routingKey: queueName,
                    body: body);
                
                _logger.LogInformation("✅ Mensagem enviada com sucesso para fila {Queue}", queueName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao publicar mensagem na fila {Queue}", queueName);
            }
        }

        /// <summary>
        /// Método moderno para publicar usando exchanges e routing keys
        /// </summary>
        public async Task PublishToExchangeAsync<T>(string exchange, string routingKey, T message) where T : class
        {
            _logger.LogInformation("🔄 Iniciando PublishToExchangeAsync para exchange {Exchange} com routing key {RoutingKey}", 
                exchange, routingKey);

            if (!_rabbitMQEnabled)
            {
                _logger.LogInformation("RabbitMQ desabilitado. Mensagem não enviada para exchange {Exchange}: {Message}", 
                    exchange, JsonSerializer.Serialize(message));
                return;
            }

            if (_channel == null)
            {
                _logger.LogWarning("❌ Canal RabbitMQ não disponível. Mensagem não enviada para exchange {Exchange}", exchange);
                return;
            }

            try
            {
                var json = JsonSerializer.Serialize(message);
                var body = Encoding.UTF8.GetBytes(json);

                _logger.LogInformation("📨 Enviando mensagem para exchange {Exchange} com routing key {RoutingKey}: {Message}", 
                    exchange, routingKey, json);

                if (_channel != null) // ✅ Verificação explícita adicional
                {
                    await _channel.BasicPublishAsync(
                        exchange: exchange,
                        routingKey: routingKey,
                        body: body);
                    
                    _logger.LogInformation("✅ Mensagem enviada com sucesso para exchange {Exchange} com routing key {RoutingKey}", 
                        exchange, routingKey);
                }
                else
                {
                    _logger.LogWarning("Canal RabbitMQ não disponível durante publicação para exchange");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao publicar mensagem no exchange {Exchange} com routing key {RoutingKey}", 
                    exchange, routingKey);
            }
        }

        public void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
        }
    }
}
