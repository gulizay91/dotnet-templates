using System.Net;
using Common.Exchanges.Response;
using WorkerConsumer.Application.Interfaces.Proxies;
using WorkerConsumer.Application.Interfaces.Services;

namespace WorkerConsumer.Infrastructure.Services;

public class TemplateService : ITemplateService
{
  private readonly ITemplateApiClient _templateApiClient;
  private readonly ILogger<TemplateService> _logger;

  public TemplateService(ITemplateApiClient templateApiClient, ILogger<TemplateService> logger)
  {
    _templateApiClient = templateApiClient;
    _logger = logger;
  }

  public async Task<ServiceResponse<string?>> GetTemplateAsync(string templateId, CancellationToken cancellationToken)
  {
    try
    {
      var response = await _templateApiClient.GetTemplateByIdAsync(templateId, cancellationToken);
      return response is null
        ? new ServiceResponse<string?>(true, (int)HttpStatusCode.NotFound, "Template not found", null, null)
        : new ServiceResponse<string?>(true, (int)HttpStatusCode.OK, "Success", null, data: response.Data);
    }
    catch (Exception e)
    {
      _logger.LogError(e.Message);
      return new ServiceResponse<string?>(false, (int)HttpStatusCode.InternalServerError, "Server Error!", null,
        null);
    }
  }
}