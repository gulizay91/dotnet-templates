namespace MessageBroker.RabbitMqBroker.Interfaces;

public interface IRabbitMQConnectionBackgroundService
{
  Task WaitForInitializationAsync();
}