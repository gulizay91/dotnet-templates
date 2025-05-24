using System.Diagnostics;
using System.Net;
using System.Text.Json;
using Common.Loggers;

namespace Common.Handlers;

public class RequestResponseLoggingHandler : DelegatingHandler
{
  private readonly IProxyLogger _logger;

  public RequestResponseLoggingHandler(IProxyLogger logger)
  {
    _logger = logger;
  }

  protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
    CancellationToken cancellationToken)
  {
    var stopwatch = Stopwatch.StartNew();

    string? requestBody = null;
    if (request.Content != null)
    {
      // Read the request body as a string
      var rawRequestBody = await request.Content.ReadAsStringAsync(cancellationToken);

      try
      {
        // Attempt to parse the string as JSON and format it nicely
        var parsedJson = JsonDocument.Parse(rawRequestBody);
        requestBody = JsonSerializer.Serialize(parsedJson, new JsonSerializerOptions { WriteIndented = true });
      }
      catch (JsonException)
      {
        // If parsing fails, log the raw request body
        requestBody = rawRequestBody;
      }
    }

    await _logger.LogInformation("HttpClient request started", null, null,
      requestBody, request.Method, null, null, request.RequestUri?.Host, request.RequestUri?.AbsolutePath);

    var response = new HttpResponseMessage(HttpStatusCode.RequestTimeout);
    try
    {
      response = await base.SendAsync(request, cancellationToken);
      
      stopwatch.Stop();

      response.EnsureSuccessStatusCode();

      var responseMessage = await response.Content.ReadAsStringAsync(cancellationToken);

      await _logger.LogInformation("HttpClient request finished",
        null, responseMessage, requestBody, request.Method,
        response.StatusCode, stopwatch.ElapsedMilliseconds, request.RequestUri?.Host,
        request.RequestUri?.AbsolutePath);
    }
    catch(Exception ex)
    {
      stopwatch.Stop();
      await _logger.LogError("HttpClient error!", ex, ex.Message,
        requestBody, request.Method, response.StatusCode, stopwatch.ElapsedMilliseconds,
        request.RequestUri?.Host, request.RequestUri?.AbsolutePath);
      return response;
    }

    return response;
  }
}