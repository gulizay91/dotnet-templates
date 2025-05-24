namespace MessageBroker.RabbitMqBroker.Configurations;

public record QueueSettings
{
  private const int DefaultRetryMaxCount = 3;
  private const int DefaultRetryDelaySecond = 3;
  
  public required string QueueName { get; init; }
  public bool HasDlq { get; init; } = false;
  public required string ExchangeType { get; init; }
  public required string ExchangeName { get; init; }
  public bool Durable { get; init; } = true;
  public int RetryMaxCount { get; init; } = DefaultRetryMaxCount;
  public int RetryDelaySecond { get; init; } = DefaultRetryDelaySecond;
}