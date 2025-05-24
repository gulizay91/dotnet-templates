using WorkerConsumer.Application.Interfaces.Services;
using WorkerConsumer.BackgroundServices;
using WorkerConsumer.Infrastructure.Services;

namespace WorkerConsumer.Registrations;

public static class ServiceRegister
{
  public static void RegisterServices(this IServiceCollection serviceCollection)
  {
    serviceCollection.AddScoped<ITemplateService, TemplateService>();
    
    serviceCollection.AddHostedService<ApplicationLifetimeService>();
  }
}