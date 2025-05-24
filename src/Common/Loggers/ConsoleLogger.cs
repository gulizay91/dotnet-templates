using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;

namespace Common.Loggers;

public class ConsoleLogger : IConsoleLogger
{
  private readonly LogLevel _logLevel;

  public ConsoleLogger(LogLevel logLevel = LogLevel.Information)
  {
    _logLevel = logLevel;
  }
  
  public async Task LogTrace(string message, object[]? args = null, Exception exception = null, long? duration = null, string className = null,
    string methodName = null, string user = null, string responseBody = null, string requestBody = null,
    Guid? correlationId = null)
  {
    await Log(LogLevel.Trace, message, args, exception, duration, className, methodName, user, responseBody, requestBody, correlationId);
  }

  public async Task LogDebug(string message, object[]? args = null, Exception exception = null, long? duration = null, string className = null,
    string methodName = null, string user = null, string responseBody = null, string requestBody = null,
    Guid? correlationId = null)
  {
    await Log(LogLevel.Debug, message, args, exception, duration, className, methodName, user, responseBody, requestBody, correlationId);
  }

  public async Task LogInformation(string message, object[]? args = null, Exception exception = null, long? duration = null, string className = null,
    string methodName = null, string user = null, string responseBody = null, string requestBody = null,
    Guid? correlationId = null)
  {
    await Log(LogLevel.Information, message, args, exception, duration, className, methodName, user, responseBody, requestBody, correlationId);
  }

  public async Task LogWarning(string message, object[]? args = null, Exception exception = null, long? duration = null, string className = null,
    string methodName = null, string user = null, string responseBody = null, string requestBody = null,
    Guid? correlationId = null)
  {
    await Log(LogLevel.Warning, message, args, exception, duration, className, methodName, user, responseBody, requestBody, correlationId);
  }

  public async Task LogError(string message, object[]? args = null, Exception exception = null, long? duration = null, string className = null,
    string methodName = null, string user = null, string responseBody = null, string requestBody = null,
    Guid? correlationId = null)
  {
    await Log(LogLevel.Error, message, args, exception, duration, className, methodName, user, responseBody, requestBody, correlationId);
  }

  public async Task LogCritical(string message, object[]? args = null, Exception exception = null, long? duration = null, string className = null,
    string methodName = null, string user = null, string responseBody = null, string requestBody = null,
    Guid? correlationId = null)
  {
    await Log(LogLevel.Critical, message, args, exception, duration, className, methodName, user, responseBody, requestBody, correlationId);
  }
  
  private async Task Log(LogLevel logLevel, string message, object[]? args = null, Exception? exception = null, long? duration = null,
    string? className = null, string? methodName = null, string? user = null,
    string? responseBody = null, string? requestBody = null,
    Guid? correlationId = null)
  {
    if (_logLevel > logLevel) return;

    Dictionary<string, object> dynamicFields = new();

    if (args is { Length: > 0 } && message.Contains('{'))
    {
      var formatKeys = ExtractFormatKeys(message);

      for (int i = 0; i < args.Length && i < formatKeys.Count; i++)
      {
        dynamicFields[formatKeys[i]] = args[i];
      }
      
      message = ReplacePlaceholders(message, formatKeys, args);
    }

    var logEntry = new Dictionary<string, object?>
    {
      ["CorrelationId"] = correlationId,
      ["DateTime"] = DateTime.UtcNow,
      ["LogLevel"] = logLevel.ToString(),
      ["Message"] = message,
      ["ExceptionType"] = exception?.GetType().Name,
      ["Exception"] = exception?.ToString(),
      ["ExceptionMessage"] = exception?.Message,
      ["StackTrace"] = exception?.StackTrace,
      ["Duration"] = duration,
      ["User"] = user,
      ["Class"] = className,
      ["Method"] = methodName,
      ["ResponseBody"] = responseBody,
      ["RequestBody"] = requestBody,
    };

    foreach (var kvp in dynamicFields)
    {
      logEntry[kvp.Key] = kvp.Value;
    }

    var logMessage = JsonSerializer.Serialize(logEntry, new JsonSerializerOptions
    {
      DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
      WriteIndented = false
    });

    await Console.Out.WriteLineAsync(logMessage);
  }
  
  private List<string> ExtractFormatKeys(string message)
  {
    var matches = System.Text.RegularExpressions.Regex.Matches(message, @"\{([^\}]+)\}");
    return matches.Select(m => m.Groups[1].Value).ToList();
  }

  private string ReplacePlaceholders(string message, List<string> keys, object[] args)
  {
    for (int i = 0; i < keys.Count && i < args.Length; i++)
    {
      var value = args[i]?.ToString() ?? string.Empty;
      message = message.Replace("{" + keys[i] + "}", value);
    }
    return message;
  }
}