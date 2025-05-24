using System.Net;
using System.Text;

namespace WorkerConsumer.BackgroundServices;

public class HttpHealthCheck : BackgroundService
{
  private readonly ILogger<HttpHealthCheck> _logger;
  private readonly HttpListener _httpListener;
  private readonly IConfiguration _configuration;

  public HttpHealthCheck(ILogger<HttpHealthCheck> logger, IConfiguration configuration)
  {
    _logger = logger;
    _configuration = configuration;
    _httpListener = new HttpListener();
  }

  protected override async Task ExecuteAsync(CancellationToken stoppingToken)
  {
    var port = _configuration.GetValue<int>("HealthCheckPort");
    _httpListener.Prefixes.Add($"http://*:{port}/health/");
    _httpListener.Prefixes.Add($"http://*:{port}/ready/");

    _httpListener.Start();
    _logger.LogInformation($"Healthcheck listening on port {port}...");

    while (!stoppingToken.IsCancellationRequested)
    {
      HttpListenerContext ctx = null;
      try
      {
        ctx = await _httpListener.GetContextAsync();
      }
      catch (HttpListenerException ex)
      {
        if (ex.ErrorCode == 995) return;
      }

      if (ctx == null) continue;

      var response = ctx.Response;
      response.ContentType = "text/plain";
      response.Headers.Add(HttpResponseHeader.CacheControl, "no-store, no-cache");
      response.StatusCode = (int)HttpStatusCode.OK;

      var messageBytes = Encoding.UTF8.GetBytes("Healthy");
      response.ContentLength64 = messageBytes.Length;
      await response.OutputStream.WriteAsync(messageBytes, 0, messageBytes.Length, stoppingToken);
      response.OutputStream.Close();
      response.Close();
    }
  }
}