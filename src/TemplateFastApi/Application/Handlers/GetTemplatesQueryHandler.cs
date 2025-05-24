using System.Net;
using TemplateFastApi.Application.Contracts;
using TemplateFastApi.Application.Contracts.Queries;
using TemplateFastApi.Application.Exchanges.Response;

namespace TemplateFastApi.Application.Handlers;

public class GetTemplatesQueryHandler: IRequestHandlerWrapper<GetTemplatesQuery, GetTemplatesResponse>
{
  private static readonly int[] Templates = [1, 2, 3, 4, 5];
  
  public async Task<GetTemplatesResponse> Handle(GetTemplatesQuery request, CancellationToken cancellationToken)
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(request.TemplateId.ToString());
    var response = await GetTemplateByIdAsync(request.TemplateId.ToString(), cancellationToken);
    return response is null
      ? new GetTemplatesResponse(response,true, (int)HttpStatusCode.NotFound, "Template not found")
      : new GetTemplatesResponse(response,true, (int)HttpStatusCode.OK, "Success");
  }
  
  private async Task<string?> GetTemplateByIdAsync(string templateId, CancellationToken cancellationToken)
  {
    if (int.TryParse(templateId, out var id) && Templates.Contains(id))
    {
      return await Task.FromResult<string?>(templateId);
    }

    return await Task.FromResult<string?>(null);
  }
}