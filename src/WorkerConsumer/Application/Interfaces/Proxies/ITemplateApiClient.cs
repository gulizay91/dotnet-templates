using Common.Exchanges.Response;

namespace WorkerConsumer.Application.Interfaces.Proxies;

public interface ITemplateApiClient
{
  Task<ServiceResponse<string>?> GetTemplateByIdAsync(string templateId, CancellationToken cancellationToken);
}