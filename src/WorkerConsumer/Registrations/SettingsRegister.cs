using WorkerConsumer.Configurations;

namespace WorkerConsumer.Registrations;

public static class SettingsRegister
{
  public static void RegisterSettings(this IServiceCollection serviceCollection, IConfiguration configuration)
  {
    serviceCollection.Configure<TemplateApiSettings>(
      configuration.GetSection("Proxies:TemplateApi"));
    
    serviceCollection.ValidateSettings(configuration);
  }

  private static void ValidateSettings(this IServiceCollection serviceCollection, IConfiguration configuration)
  {
    serviceCollection.AddOptions<TemplateApiSettings>()
      .Bind(configuration.GetSection("Proxies:TemplateApi"))
      .ValidateDataAnnotations()
      .ValidateOnStart();
  }
}