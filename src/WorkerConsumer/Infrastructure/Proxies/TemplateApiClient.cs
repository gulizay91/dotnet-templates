using System.Net;
using Common.Clients;
using Common.Exchanges.Response;
using Microsoft.Extensions.Options;
using WorkerConsumer.Application.Interfaces.Proxies;
using WorkerConsumer.Configurations;

namespace WorkerConsumer.Infrastructure.Proxies;

public class TemplateApiClient : BaseHttpClient, ITemplateApiClient
{
  private readonly ILogger<TemplateApiClient> _logger;
  private readonly TemplateApiSettings _templateApiSettings;

  public TemplateApiClient(HttpClient httpClient, IOptions<TemplateApiSettings> templateApiSettings, ILogger<TemplateApiClient> logger) : base(httpClient)
  {
    _templateApiSettings = templateApiSettings.Value;
    _logger = logger;
  }

  public async Task<ServiceResponse<string>?> GetTemplateByIdAsync(string templateId, CancellationToken cancellationToken)
  {
    try
    {
      var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"api/v1/templates/{templateId}");
      return await GetAsync<ServiceResponse<string>>(requestMessage);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error getting template info from template api for templateId {0}", templateId);
      return new ServiceResponse<string>(false, (int)HttpStatusCode.InternalServerError, ex.Message, string.Empty, null);
    }
  }
}