using Common.Configurations;
using Common.Handlers;
using WorkerConsumer.Application.Interfaces.Proxies;
using WorkerConsumer.Infrastructure.Proxies;

namespace WorkerConsumer.Registrations;

public static class HttpClientRegister
{
  public static void RegisterHttpClients(this IServiceCollection serviceCollection, IConfiguration configuration)
  {
    serviceCollection.AddTransient<RequestResponseLoggingHandler>();
    
    serviceCollection.AddTemplateApiClient(configuration);
  }
  
  private static void AddTemplateApiClient(this IServiceCollection serviceCollection, IConfiguration configuration)
  {
    var proxySettings = new ProxySettings();
    configuration.GetSection("Proxies:TemplateApi:ProxySettings").Bind(proxySettings);

    serviceCollection.AddHttpClient<ITemplateApiClient, TemplateApiClient>(
        nameof(ITemplateApiClient),
        client =>
        {
          client.Timeout = TimeSpan.FromSeconds(proxySettings.Timeout);
          client.BaseAddress = new Uri(proxySettings.Url);
        })
      .SetHandlerLifetime(TimeSpan.FromMinutes(5))
      .AddHttpMessageHandler<RequestResponseLoggingHandler>()
      .AddCustomResilienceHandler(proxySettings);
  }
}