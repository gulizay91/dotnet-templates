using System.Net;
using Common.Configurations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;
using Polly;

namespace Common.Handlers;

public static class CustomResilienceHandler
{
  public static void AddCustomResilienceHandler(this IHttpClientBuilder httpClientBuilder, ProxySettings proxySettings)
  {
    httpClientBuilder.AddResilienceHandler(
      "ResiliencePipeline",
      builder =>
      {
        builder.AddConcurrencyLimiter(100);

        builder.AddRetry(new HttpRetryStrategyOptions
        {
          BackoffType = DelayBackoffType.Exponential,
          MaxRetryAttempts = proxySettings.RetryAttempts,
        });

        builder.AddCircuitBreaker(new HttpCircuitBreakerStrategyOptions
        {
          SamplingDuration = TimeSpan.FromSeconds(10),
          FailureRatio = 0.2,
          MinimumThroughput = 3,
          ShouldHandle = static args =>
          {
            return ValueTask.FromResult(args is
            {
              Outcome.Result.StatusCode:
              HttpStatusCode.RequestTimeout or
              HttpStatusCode.TooManyRequests
            });
          }
        });

        builder.AddTimeout(TimeSpan.FromSeconds(proxySettings.Timeout));
      });
  }
}