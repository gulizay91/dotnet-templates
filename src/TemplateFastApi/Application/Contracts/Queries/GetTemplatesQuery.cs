using TemplateFastApi.Application.Exchanges.Response;

namespace TemplateFastApi.Application.Contracts.Queries;

public record GetTemplatesQuery: IRequestWrapper<GetTemplatesResponse>
{
  public int TemplateId { get; set; }
}