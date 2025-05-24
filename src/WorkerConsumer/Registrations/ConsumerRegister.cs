using MessageBroker.RabbitMqBroker.Registrations;
using WorkerConsumer.Consumers;

namespace WorkerConsumer.Registrations;

public static class ConsumerRegister
{
  /// <summary>
  /// Make sure each consumer should register after rabbitmq!
  /// </summary>
  /// <param name="serviceCollection"></param>
  /// <param name="configuration"></param>
  public static void RegisterConsumer(this IServiceCollection serviceCollection, IConfiguration configuration)
  {
    serviceCollection.RegisterRabbitMQ(configuration);
    serviceCollection.AddHostedService<TemplateConsumer>();
  }
}