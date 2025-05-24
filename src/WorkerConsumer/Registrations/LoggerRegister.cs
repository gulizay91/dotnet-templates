using System.Text.Json;
using Common.Loggers;

namespace WorkerConsumer.Registrations;

public static class LoggerRegister
{
  public static void RegisterLoggers(this IServiceCollection serviceCollection, IConfiguration configuration)
  {
    var defaultLogLevel = LogLevel.Error;
    if (!string.IsNullOrWhiteSpace(configuration.GetSection("Logging:LogLevel:Default").Value))
      Enum.TryParse(configuration.GetSection("Logging:LogLevel:Default").Value, true, out defaultLogLevel);
    Console.Out.WriteLine($"Console:LogLevel:Default: {defaultLogLevel}");
    serviceCollection.AddLogging(loggingBuilder =>
    {
      loggingBuilder.SetMinimumLevel(defaultLogLevel);
      loggingBuilder.AddJsonConsole(options =>
      {
        options.IncludeScopes = false;
        options.UseUtcTimestamp = true;
        options.TimestampFormat = "[yyyy-MM-dd HH:mm:ssZ]";
        options.JsonWriterOptions = new JsonWriterOptions
        {
          Indented = false
        };
      });
    });
    
    serviceCollection.AddSingleton<IProxyLogger>(sp =>
    {
      var proxyLogLevel = LogLevel.Information;
      if (!string.IsNullOrWhiteSpace(configuration.GetSection("Logging:LogLevel:ProxyLogger").Value))
        Enum.TryParse(configuration.GetSection("Logging:LogLevel:ProxyLogger").Value, true, out proxyLogLevel);
      return new ProxyLogger(proxyLogLevel);
    });
    
    serviceCollection.AddSingleton<IConsoleLogger>(sp =>
    {
      var consoleLogLevel = LogLevel.Information;
      if (!string.IsNullOrWhiteSpace(configuration.GetSection("Logging:LogLevel:ConsoleLogger").Value))
        Enum.TryParse(configuration.GetSection("Logging:LogLevel:ConsoleLogger").Value, true, out consoleLogLevel);
      return new ConsoleLogger(consoleLogLevel);
    });
  }
}