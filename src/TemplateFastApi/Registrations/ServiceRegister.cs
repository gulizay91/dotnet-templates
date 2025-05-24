using TemplateFastApi.WorkerServices;

namespace TemplateFastApi.Registrations;

public static class ServiceRegister
{
  public static void RegisterServices(this IServiceCollection serviceCollection)
  {
    serviceCollection.AddHostedService<ApplicationLifetimeService>();
  }
}