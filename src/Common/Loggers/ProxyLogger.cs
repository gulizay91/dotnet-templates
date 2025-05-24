using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;

namespace Common.Loggers;

public class ProxyLogger : IProxyLogger
{
  private readonly LogLevel _logLevel;

  public ProxyLogger(LogLevel logLevel = LogLevel.Information)
  {
    _logLevel = logLevel;
  }

  public async Task LogTrace(string message, Exception? exception = null, string? responseBody = null,
    string? requestBody = null,
    HttpMethod? httpMethod = null, HttpStatusCode? httpStatusCode = null, long? duration = null,
    string? hostName = null,
    string? url = null, string? origin = null, Guid? correlationId = null)
  {
    await Log(LogLevel.Trace, message, exception, responseBody, requestBody, httpMethod,
      httpStatusCode.GetValueOrDefault(), duration, hostName, url, origin, correlationId);
  }

  public async Task LogDebug(string message, Exception? exception = null, string? responseBody = null,
    string? requestBody = null,
    HttpMethod? httpMethod = null, HttpStatusCode? httpStatusCode = null, long? duration = null,
    string? hostName = null,
    string? url = null, string? origin = null, Guid? correlationId = null)
  {
    await Log(LogLevel.Debug, message, exception, responseBody, requestBody, httpMethod,
      httpStatusCode.GetValueOrDefault(), duration, hostName, url, origin, correlationId);
  }

  public async Task LogInformation(string message, Exception? exception = null, string? responseBody = null,
    string? requestBody = null,
    HttpMethod? httpMethod = null, HttpStatusCode? httpStatusCode = null, long? duration = null,
    string? hostName = null,
    string? url = null, string? origin = null, Guid? correlationId = null)
  {
    await Log(LogLevel.Information, message, exception, responseBody, requestBody, httpMethod,
      httpStatusCode.GetValueOrDefault(), duration, hostName, url, origin, correlationId);
  }

  public async Task LogWarning(string message, Exception? exception = null, string? responseBody = null,
    string? requestBody = null,
    HttpMethod? httpMethod = null, HttpStatusCode? httpStatusCode = null, long? duration = null,
    string? hostName = null,
    string? url = null, string? origin = null, Guid? correlationId = null)
  {
    await Log(LogLevel.Warning, message, exception, responseBody, requestBody, httpMethod,
      httpStatusCode.GetValueOrDefault(), duration, hostName, url, origin, correlationId);
  }

  public async Task LogError(string message, Exception? exception = null, string? responseBody = null,
    string? requestBody = null,
    HttpMethod? httpMethod = null, HttpStatusCode? httpStatusCode = null, long? duration = null,
    string? hostName = null,
    string? url = null, string? origin = null, Guid? correlationId = null)
  {
    await Log(LogLevel.Error, message, exception, responseBody, requestBody, httpMethod,
      httpStatusCode.GetValueOrDefault(), duration, hostName, url, origin, correlationId);
  }

  public async Task LogCritical(string message, Exception? exception = null, string? responseBody = null,
    string? requestBody = null,
    HttpMethod? httpMethod = null, HttpStatusCode? httpStatusCode = null, long? duration = null,
    string? hostName = null,
    string? url = null, string? origin = null, Guid? correlationId = null)
  {
    await Log(LogLevel.Critical, message, exception, responseBody, requestBody, httpMethod,
      httpStatusCode.GetValueOrDefault(), duration, hostName, url, origin, correlationId);
  }

  private async Task Log(LogLevel logLevel, string message, Exception? exception, string? responseBody,
    string? requestBody,
    HttpMethod? httpMethod, HttpStatusCode httpStatusCode, long? duration, string? hostName, string? url,
    string? origin, Guid? correlationId)
  {
    if (_logLevel > logLevel) return;

    var logEntry = new
    {
      CorrelationId = correlationId,
      DateTime = DateTime.UtcNow,
      LogLevel = logLevel.ToString(),
      Message = message,
      ExceptionType = exception?.GetType().Name,
      Exception = exception?.ToString(),
      ResponseBody = responseBody,
      RequestBody = requestBody,
      HttpMethod = httpMethod?.Method,
      HttpStatusCode = (int)httpStatusCode,
      Duration = duration,
      HostName = hostName,
      Url = url,
      Origin = origin
    };

    var logMessage = JsonSerializer.Serialize(logEntry, new JsonSerializerOptions
    {
      DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    });

    await Console.Out.WriteLineAsync(logMessage);
  }
}