namespace MessageBroker.RabbitMqBroker.Interfaces;

public interface IRabbitMQClient
{
  Task InitializeAsync();
  Task EnsureConnectionAsync();
  Task PublishMessageAsync(string exchangeName, string message, CancellationToken cancellationToken = default);
  Task ConsumeMessageAsync(string queueName,
    Func<string, Task<bool>> processMessage,
    int retryMaxCount = 3,
    int retryDelaySeconds = 5,
    CancellationToken cancellationToken = default);
  Task DeclareExchangeAndQueueAsync(
    string exchangeName,
    string queueName,
    string exchangeType = "fanout",
    bool durable = true,
    bool hasDlq = false);

  Task DeclareQueueAsync(string name, bool durable, bool exclusive, bool autoDelete,
    IDictionary<string, object?>? arguments = null, bool passive = false, bool noWait = false,
    CancellationToken cancellationToken = default, bool hasDlq = false);
  ValueTask DisposeAsync();
}