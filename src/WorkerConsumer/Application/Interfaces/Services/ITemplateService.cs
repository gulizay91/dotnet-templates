using Common.Exchanges.Response;

namespace WorkerConsumer.Application.Interfaces.Services;

public interface ITemplateService
{
  Task<ServiceResponse<string?>> GetTemplateAsync(string templateId, CancellationToken cancellationToken);
}