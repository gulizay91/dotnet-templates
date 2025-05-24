using System.Text;
using MessageBroker.RabbitMqBroker.Configurations;
using MessageBroker.RabbitMqBroker.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace MessageBroker.RabbitMqBroker.Implementations;

public class RabbitMQClient : IRabbitMQClient, IAsyncDisposable
{
    private readonly ILogger<RabbitMQClient> _logger;
    private readonly ConnectionFactory _connectionFactory;
    private IConnection? _connection;
    private IChannel? _channel;
    
    private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
    private const string DeadLetterQueueSuffix = ".dlq";
    private const string HeaderRetryCountKey = "x-retry-count";

    public RabbitMQClient(IOptions<RabbitMQSettings> rabbitMqSettings, ILogger<RabbitMQClient> logger)
    {
        var settings = rabbitMqSettings.Value;
        _connectionFactory = new ConnectionFactory
        {
            HostName = settings.Hostname,
            UserName = settings.Username,
            Password = settings.Password
        };
        _logger = logger;
    }

    public async Task InitializeAsync()
    {
        if (_connection is not { IsOpen: true })
        {
            _logger.LogInformation("Initializing RabbitMQ connection...");
            _connection = await _connectionFactory.CreateConnectionAsync();
        }

        if (_channel is not { IsOpen: true })
        {
            _logger.LogInformation("Creating RabbitMQ channel...");
            _channel = await _connection.CreateChannelAsync();
        }

        _logger.LogInformation("RabbitMQ connection and channel established.");
    }

    public async Task EnsureConnectionAsync()
    {
        if (_connection == null)
            await InitializeAsync();
        else if (_connection is not { IsOpen: true })
        {
            _logger.LogWarning("RabbitMQ connection lost. Reconnecting...");
            _connection?.Dispose();
            _connection = await _connectionFactory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();
            _logger.LogInformation("RabbitMQ connection re-established.");
        }
    }

    public async Task DeclareExchangeAndQueueAsync(
        string exchangeName,
        string queueName,
        string exchangeType = ExchangeType.Fanout,
        bool durable = true,
        bool hasDlq = false)
    {
        await _semaphore.WaitAsync();
        
        try
        {
            await EnsureConnectionAsync();
            ArgumentNullException.ThrowIfNull(_channel);
            await _channel.ExchangeDeclareAsync(exchangeName, exchangeType, durable);

            if (hasDlq)
            {
                var dlqName = $"{queueName}{DeadLetterQueueSuffix}";
                await DeclareQueueAsync(dlqName, true, false, false, hasDlq: false);
            }

            await DeclareQueueAsync(queueName, durable, false, false, hasDlq: hasDlq);

            await _channel.QueueBindAsync(queueName, exchangeName, string.Empty);

            _logger.LogInformation($"Exchange '{exchangeName}' and Queue '{queueName}' declared.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to declare exchange {exchangeName} and queue {queueName}");
            throw;
        }
        finally
        {
            _semaphore.Release();
        }
    }
    
    public async Task DeclareQueueAsync(string name, bool durable, bool exclusive, bool autoDelete,
        IDictionary<string, object?>? arguments = null, bool passive = false, bool noWait = false,
        CancellationToken cancellationToken = default, bool hasDlq = false)
    {
        await EnsureConnectionAsync();
        if (hasDlq)
        {
            arguments ??= new Dictionary<string, object?>();
            arguments["x-dead-letter-exchange"] = string.Empty; // Default Exchange
            arguments["x-dead-letter-routing-key"] = name + DeadLetterQueueSuffix;
        }
        
        try
        {
            ArgumentNullException.ThrowIfNull(_channel);
            await _channel.QueueDeclareAsync(
                queue: name,
                durable: durable,
                exclusive: exclusive,
                autoDelete: autoDelete,
                arguments: arguments,
                passive: passive,
                noWait: noWait, cancellationToken: cancellationToken);

            _logger.LogInformation($"Queue declared: {name}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to declare queue {name}");
        }
    }

    public async Task PublishMessageAsync(string exchangeName, string message, CancellationToken cancellationToken = default)
    {
        var body = Encoding.UTF8.GetBytes(message);
        ArgumentNullException.ThrowIfNull(_channel);
        await _channel.BasicPublishAsync(exchange: exchangeName, routingKey: string.Empty, body: body, cancellationToken: cancellationToken);
        _logger.LogInformation($"Message published to exchange '{exchangeName}': {message}");
    }
    
    public async Task ConsumeMessageAsync(
        string queueName,
        Func<string, Task<bool>> processMessage,
        int retryMaxCount = 3,
        int retryDelaySeconds = 5,
        CancellationToken cancellationToken = default)
    {
        if (_channel is not { IsOpen: true })
        {
            _logger.LogWarning("RabbitMQ channel is not initialized or open. Reinitializing...");
            await EnsureConnectionAsync();
        }
        
        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            var message = Encoding.UTF8.GetString(ea.Body.ToArray());
            try
            {
                _logger.LogInformation($"Message received: {message}");
                var success = await processMessage(message);

                if (success)
                {
                    await _channel.BasicAckAsync(ea.DeliveryTag, false, cancellationToken);
                    _logger.LogInformation("Message processed successfully.");
                }
                else
                {
                    await RetryOrSendToDlqAsync(queueName, ea, retryMaxCount, retryDelaySeconds, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message. Retry Or Send To DLQ.");
                await RetryOrSendToDlqAsync(queueName, ea, retryMaxCount, retryDelaySeconds, cancellationToken);
            }
        };

        await _channel.BasicConsumeAsync(queue: queueName, autoAck: false, consumer: consumer, cancellationToken: cancellationToken);
        _logger.LogInformation($"Started consuming messages from queue '{queueName}'.");
    }

    private async Task RetryOrSendToDlqAsync(string queueName, BasicDeliverEventArgs ea, int retryMaxCount, int retryDelaySeconds, CancellationToken cancellationToken = default)
    {
        var headers = ea.BasicProperties.Headers ?? new Dictionary<string, object>();
        var retryCount = headers.TryGetValue(HeaderRetryCountKey, out var count) ? Convert.ToInt32(count) : 0;

        if (retryCount < retryMaxCount)
        {
            headers[HeaderRetryCountKey] = retryCount + 1;
            await Task.Delay(TimeSpan.FromSeconds(retryDelaySeconds), cancellationToken);
            ArgumentNullException.ThrowIfNull(_channel);
            await _channel.BasicPublishAsync(
                exchange: string.Empty,
                routingKey: queueName,
                mandatory: false,
                basicProperties: new BasicProperties { Headers = headers },
                body: ea.Body, cancellationToken: cancellationToken);

            _logger.LogInformation($"Retrying message. Retry count: {retryCount + 1}");
        }
        else
        {
            var dlqName = queueName + DeadLetterQueueSuffix;
            ArgumentNullException.ThrowIfNull(_channel);
            await _channel.QueueDeclareAsync(dlqName, durable: true, exclusive: false, autoDelete: false, cancellationToken: cancellationToken);
            await _channel.BasicPublishAsync(exchange: string.Empty, routingKey: dlqName, body: ea.Body, cancellationToken: cancellationToken);
            _logger.LogInformation($"Message sent to DLQ: {dlqName}");
        }

        await _channel.BasicAckAsync(ea.DeliveryTag, false, cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        if (_channel != null && _channel.IsOpen)
        {
            await _channel.CloseAsync();
            await _channel.DisposeAsync();
        }

        if (_connection != null && _connection.IsOpen)
        {
            await _connection.CloseAsync();
            await _connection.DisposeAsync();
        }

        _logger.LogInformation("RabbitMQ connection closed.");
        GC.SuppressFinalize(this);
    }
}