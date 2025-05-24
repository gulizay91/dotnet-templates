namespace WorkerConsumer.Registrations;

public static class ConfigurationRegister
{
  public static void RegisterConfiguration(this IHostApplicationBuilder applicationBuilder)
  {
    applicationBuilder.Configuration.AddConfiguration(LoadConfiguration());
  }

  private static IConfiguration LoadConfiguration()
  {
    var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
    var configBuilder = new ConfigurationBuilder()
      .SetBasePath(Directory.GetCurrentDirectory())
      .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
    
    if (!string.IsNullOrWhiteSpace(environment))
    {
      configBuilder.AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true);
    }
    configBuilder.AddEnvironmentVariables();
    
    return configBuilder.Build();
  }
}