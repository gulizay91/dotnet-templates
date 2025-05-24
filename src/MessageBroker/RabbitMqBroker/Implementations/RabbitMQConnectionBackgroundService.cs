using MessageBroker.RabbitMqBroker.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MessageBroker.RabbitMqBroker.Implementations;

public class RabbitMQConnectionBackgroundService : BackgroundService, IRabbitMQConnectionBackgroundService
{
  private readonly IRabbitMQClient _rabbitMqClient;
  private readonly IRabbitMQConfiguration _rabbitMQConfiguration;
  private readonly ILogger<RabbitMQConnectionBackgroundService> _logger;
  private readonly TaskCompletionSource<bool> _initializationCompleted = new();

  public RabbitMQConnectionBackgroundService(IRabbitMQClient rabbitMqClient, ILogger<RabbitMQConnectionBackgroundService> logger, IRabbitMQConfiguration rabbitMQConfiguration)
  {
    _rabbitMqClient = rabbitMqClient;
    _logger = logger;
    _rabbitMQConfiguration = rabbitMQConfiguration;
  }

  public Task WaitForInitializationAsync() => _initializationCompleted.Task;

  private async Task EnsureExchangeAndQueueAsync()
  {
    foreach (var (consumerKey, consumerSettings) in _rabbitMQConfiguration.Consumers)
    {
      await _rabbitMqClient.DeclareExchangeAndQueueAsync(
        consumerSettings.ExchangeName,
        consumerSettings.QueueName,
        consumerSettings.ExchangeType,
        consumerSettings.Durable,
        consumerSettings.HasDlq
      );

      _logger.LogInformation($"Declared queue '{consumerSettings.QueueName}' and exchange '{consumerSettings.ExchangeName}' for consumer '{consumerKey}'.");
    }
    
    _initializationCompleted.SetResult(true);
  }
  
  public override async Task StartAsync(CancellationToken cancellationToken)
  {
    _logger.LogInformation($"{nameof(RabbitMQConnectionBackgroundService)} is starting.");
    
    await _rabbitMqClient.InitializeAsync();
    
    await EnsureExchangeAndQueueAsync();

    _logger.LogInformation("RabbitMQ connection initialized.");
    
    await base.StartAsync(cancellationToken);
  }
  
  protected override async Task ExecuteAsync(CancellationToken stoppingToken)
  {
    _logger.LogInformation($"{nameof(RabbitMQConnectionBackgroundService)} is starting.");

    while (!stoppingToken.IsCancellationRequested)
    {
      try
      {
        await _rabbitMqClient.EnsureConnectionAsync();
        _logger.LogInformation("RabbitMQ connection established and healthy.");
        await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "RabbitMQ connection failed. Retrying in 5 seconds...");
        await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
      }
    }
  }

  public override async Task StopAsync(CancellationToken cancellationToken)
  {
    _logger.LogInformation($"{nameof(RabbitMQConnectionBackgroundService)} is stopping.");
    await _rabbitMqClient.DisposeAsync();
    _logger.LogInformation("RabbitMQ connection closed gracefully.");
    await base.StopAsync(cancellationToken);
  }
}