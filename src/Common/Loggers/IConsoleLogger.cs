namespace Common.Loggers;

public interface IConsoleLogger
{
  Task LogTrace(string message, object[]? args = null, Exception? exception = null, long? duration = null,
    string? className = null, string? methodName = null, string? user = null,
    string? responseBody = null, string? requestBody = null,
    Guid? correlationId = null);

  Task LogDebug(string message, object[]? args = null, Exception? exception = null, long? duration = null,
  string? className = null, string? methodName = null, string? user = null,
  string? responseBody = null, string? requestBody = null,
    Guid? correlationId = null);

  Task LogInformation(string message, object[]? args = null, Exception? exception = null, long? duration = null,
    string? className = null, string? methodName = null, string? user = null,
    string? responseBody = null, string? requestBody = null,
    Guid? correlationId = null);

  Task LogWarning(string message, object[]? args = null, Exception? exception = null, long? duration = null,
    string? className = null, string? methodName = null, string? user = null,
    string? responseBody = null, string? requestBody = null,
    Guid? correlationId = null);

  Task LogError(string message, object[]? args = null, Exception? exception = null, long? duration = null,
    string? className = null, string? methodName = null, string? user = null,
    string? responseBody = null, string? requestBody = null,
    Guid? correlationId = null);

  Task LogCritical(string message, object[]? args = null, Exception? exception = null, long? duration = null,
    string? className = null, string? methodName = null, string? user = null,
    string? responseBody = null, string? requestBody = null,
    Guid? correlationId = null);
}