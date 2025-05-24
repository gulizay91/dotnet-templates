using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace TemplateFastApi.Registrations;

public static class HealthCheckRegister
{
  public static void RegisterHealthChecks(this IServiceCollection services)
  {
    services.AddHealthChecks()
      .AddCheck("Service Self-Check", () => HealthCheckResult.Healthy("Service is running"));
  }

  public static void UseHealthCheckEndpoints(this IApplicationBuilder applicationBuilder)
  {
    //for liveness probe
    applicationBuilder.UseEndpoints(endpoints =>
    {
      endpoints.MapHealthChecks("/health", new HealthCheckOptions
      {
        Predicate = _ => false
      });
    });

    //for readiness probe 
    applicationBuilder.UseEndpoints(endpoints =>
    {
      endpoints.MapHealthChecks("/ready", new HealthCheckOptions
      {
        Predicate = check => check.Tags.Contains("ready")
      });
    });
  }
}