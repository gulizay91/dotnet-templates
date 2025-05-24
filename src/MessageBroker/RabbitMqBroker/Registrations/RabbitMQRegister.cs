using MessageBroker.RabbitMqBroker.Configurations;
using MessageBroker.RabbitMqBroker.Implementations;
using MessageBroker.RabbitMqBroker.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MessageBroker.RabbitMqBroker.Registrations;

public static class RabbitMQRegister
{
  public static void RegisterRabbitMQ(this IServiceCollection services, IConfiguration configuration)
  {
    services.RegisterSettings(configuration);
    services.AddSingleton<IRabbitMQConfiguration, RabbitMQConfiguration>();
    services.AddSingleton<IRabbitMQClient, RabbitMQClient>();
    
    services.AddSingleton<RabbitMQConnectionBackgroundService>(); 
    services.AddHostedService(provider => provider.GetRequiredService<RabbitMQConnectionBackgroundService>());
    services.AddSingleton<IRabbitMQConnectionBackgroundService>(provider => 
      provider.GetRequiredService<RabbitMQConnectionBackgroundService>());
  }
  
  private static void RegisterSettings(this IServiceCollection serviceCollection, IConfiguration configuration)
  {
    serviceCollection.Configure<RabbitMQSettings>(
      configuration.GetSection("RabbitMQSettings"));
    
    serviceCollection.ValidateSettings(configuration);
  }

  private static void ValidateSettings(this IServiceCollection serviceCollection, IConfiguration configuration)
  {
    serviceCollection.AddOptions<RabbitMQSettings>()
      .Bind(configuration.GetSection("RabbitMQSettings"))
      .ValidateDataAnnotations()
      .ValidateOnStart();
  }
}