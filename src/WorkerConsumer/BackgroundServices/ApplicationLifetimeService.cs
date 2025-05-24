namespace WorkerConsumer.BackgroundServices;

public class ApplicationLifetimeService : IHostedService
{
  private readonly IHostApplicationLifetime _applicationLifetime;
  private readonly ILogger<ApplicationLifetimeService> _logger;
  private readonly IServiceProvider _servicesProvider;

  public ApplicationLifetimeService(
    IHostApplicationLifetime applicationLifetime,
    ILogger<ApplicationLifetimeService> logger,
    IServiceProvider servicesProvider)
  {
    _applicationLifetime = applicationLifetime;
    _logger = logger;
    _servicesProvider = servicesProvider;
  }
  
  public Task StartAsync(CancellationToken cancellationToken)
  {
    _applicationLifetime.ApplicationStarted.Register(async () =>
    {
      _logger.LogInformation("Application started.");
    });
    
    // register a callback that sleeps for 30 seconds
    _applicationLifetime.ApplicationStopping.Register(() =>
    {
      _logger.LogInformation("SIGTERM received, waiting for 30 seconds");
      Thread.Sleep(30_000);
      _logger.LogInformation("Termination delay complete, continuing stopping process");
    });
    return Task.CompletedTask;
  }
  
  public Task StopAsync(CancellationToken cancellationToken)
  {
    return Task.CompletedTask;
  }
}