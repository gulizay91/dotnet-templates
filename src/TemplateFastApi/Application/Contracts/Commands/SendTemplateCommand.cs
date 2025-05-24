using TemplateFastApi.Application.Exchanges.Response;

namespace TemplateFastApi.Application.Contracts.Commands;

public record SendTemplateCommand: IRequestWrapper<SendTemplateResponse>
{
  public required int TemplateId { get; init; }
}