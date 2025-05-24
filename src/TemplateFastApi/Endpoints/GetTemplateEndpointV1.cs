using FastEndpoints;
using MediatR;
using TemplateFastApi.Application.Contracts.Queries;
using TemplateFastApi.Application.Exchanges.Response;

namespace TemplateFastApi.Endpoints;

public class GetTemplateEndpointV1 : Endpoint<EmptyRequest, GetTemplatesResponse>
{
  private readonly IMediator _mediator;

  public GetTemplateEndpointV1(IMediator mediator)
  {
    _mediator = mediator;
  }

  public override void Configure()
  {
    Get("/templates/{templateId}");
    Version(1);
    AllowAnonymous();
  }

  public override async Task HandleAsync(EmptyRequest _, CancellationToken ct)
  {
    var id = Route<int>("templateId");
    var req = new GetTemplatesQuery() { TemplateId = id };
    var response = await _mediator.Send(req, ct);
    await SendAsync(response, cancellation: ct);
  }
}