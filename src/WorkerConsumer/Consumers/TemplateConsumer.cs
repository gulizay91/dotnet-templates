using Common.Contracts.Commands;
using Common.Loggers;
using MessageBroker.RabbitMqBroker.Interfaces;
using WorkerConsumer.Application.Interfaces.Services;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace WorkerConsumer.Consumers;

public class TemplateConsumer : BackgroundService, IConsumer<CreateTemplateCommand>
{
  private readonly IRabbitMQClient _rabbitMQClient;
  private readonly IRabbitMQConfiguration _config;
  private readonly IConsoleLogger _logger;
  private readonly IRabbitMQConnectionBackgroundService _rabbitMQConnectionBackgroundService;
  private readonly IServiceScopeFactory _serviceScopeFactory; 

  public TemplateConsumer(IRabbitMQClient rabbitMQClient, IConsoleLogger logger,
    IRabbitMQConfiguration config, IServiceScopeFactory serviceScopeFactory, IRabbitMQConnectionBackgroundService rabbitMQConnectionBackgroundService)
  {
    _rabbitMQClient = rabbitMQClient;
    _logger = logger;
    _config = config;
    _serviceScopeFactory = serviceScopeFactory;
    _rabbitMQConnectionBackgroundService = rabbitMQConnectionBackgroundService;
  }

  public override async Task StartAsync(CancellationToken cancellationToken)
  {
    var queueName = _config.GetConsumerQueueName(nameof(TemplateConsumer));

    if (string.IsNullOrWhiteSpace(queueName))
    {
      await _logger.LogError($"Queue name is not configured for {nameof(TemplateConsumer)}. Stopping consumer.", className: nameof(TemplateConsumer), methodName: nameof(StartAsync));
      throw new InvalidOperationException("Queue name cannot be null or empty.");
    }

    await _rabbitMQConnectionBackgroundService.WaitForInitializationAsync();
    await _logger.LogInformation($"Starting TemplateConsumer for queue: {queueName}", className: nameof(TemplateConsumer), methodName: nameof(StartAsync));
    await base.StartAsync(cancellationToken);
  }

  protected override async Task ExecuteAsync(CancellationToken stoppingToken)
  {
    var queueName = _config.GetConsumerQueueName(nameof(TemplateConsumer));
    var retryMaxCount = _config.GetConsumerRetryMaxCount(nameof(TemplateConsumer));
    var retryDelaySeconds = _config.GetConsumerRetryDelaySeconds(nameof(TemplateConsumer));
    
    try
    {
      await _rabbitMQClient.ConsumeMessageAsync(queueName, async message =>
      {
        var messageObj = JsonSerializer.Deserialize<CreateTemplateCommand>(message);
        if (messageObj == null)
        {
          await _logger.LogWarning("Received invalid message. Skipping.", className: nameof(TemplateConsumer), methodName: nameof(ExecuteAsync));
          return false;
        }
        return await ConsumeAsync(messageObj, stoppingToken);
      },
      retryMaxCount: retryMaxCount, retryDelaySeconds: retryDelaySeconds, cancellationToken: stoppingToken);
    }
    catch (Exception ex)
    {
      await _logger.LogError($"An unexpected error occurred in the {nameof(TemplateConsumer)}.", exception: ex, className: nameof(TemplateConsumer), methodName: nameof(ExecuteAsync));
    }
  }
  
  public async Task<bool> ConsumeAsync(CreateTemplateCommand message, CancellationToken cancellationToken)
  {
    await _logger.LogInformation($"Processing message: {message}", className: nameof(TemplateConsumer), methodName: nameof(ConsumeAsync));

    try
    {
      using var scope = _serviceScopeFactory.CreateScope();
      var scopeService =
        scope.ServiceProvider.GetRequiredService<ITemplateService>();
      var serviceResponse = await scopeService.GetTemplateAsync(message.TemplateId, cancellationToken);
      if (!serviceResponse.Success)
      {
        await _logger.LogError(
          "Server Error! TemplateId: {message.TemplateId}",
          [message.TemplateId], className: nameof(TemplateConsumer), methodName: nameof(ConsumeAsync));
        return false;
      }
      if (!string.IsNullOrWhiteSpace(serviceResponse.Data))
      {
        await _logger.LogWarning(
          "Template already has been created. Skip it: {message.TemplateId}",
          [message.TemplateId], className: nameof(TemplateConsumer), methodName: nameof(ConsumeAsync));
        return true;
      }

      await _logger.LogInformation(
        "New Template message has been received. TemplateId: {message.TemplateId}",
        [message.TemplateId], className: nameof(TemplateConsumer), methodName: nameof(ConsumeAsync));

      return true;
    }
    catch (Exception ex)
    {
      await _logger.LogError("Error processing message.", exception: ex, className: nameof(TemplateConsumer), methodName: nameof(ConsumeAsync));
      return false;
    }
  }
  
  public override async Task StopAsync(CancellationToken cancellationToken)
  {
    await _logger.LogInformation("{TemplateConsumer} is stopping.", [nameof(TemplateConsumer)]);
    await _rabbitMQClient.DisposeAsync();
    await base.StopAsync(cancellationToken);
  }
}