using Microsoft.Extensions.Diagnostics.HealthChecks;
using WorkerConsumer.BackgroundServices;

namespace WorkerConsumer.Registrations;

public static class HealthCheckRegister
{
  public static void RegisterHealthChecks(this IServiceCollection serviceCollection)
  {
    serviceCollection.AddHealthChecks()
      .AddCheck("Service Self-Check", () => HealthCheckResult.Healthy("Service is running"));
    serviceCollection.AddHostedService<HttpHealthCheck>();
  }
}