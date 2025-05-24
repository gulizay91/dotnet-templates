namespace MessageBroker.RabbitMqBroker.Configurations;

public record ConsumerSettings : QueueSettings
{
  public string? Name { get; init; }
}