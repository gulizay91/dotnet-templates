namespace MessageBroker.RabbitMqBroker.Configurations;

public record PublisherSettings
{
  public string? Name { get; init; }
  public string? QueueName { get; init; }
  public string? ExchangeName { get; init; }
}