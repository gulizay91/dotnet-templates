using FastEndpoints;
using MediatR;
using TemplateFastApi.Application.Contracts.Commands;
using TemplateFastApi.Application.Exchanges.Response;

namespace TemplateFastApi.Endpoints;

public class PostTemplateEndpointV1 : Endpoint<SendTemplateCommand, SendTemplateResponse>
{
  private readonly IMediator _mediator;

  public PostTemplateEndpointV1(IMediator mediator)
  {
    _mediator = mediator;
  }

  public override void Configure()
  {
    Post("/templates");
    Version(1);
    AllowAnonymous();
  }

  public override async Task HandleAsync(SendTemplateCommand req, CancellationToken ct)
  {
    var response = await _mediator.Send(req, ct);
    await SendAsync(response, cancellation: ct);
  }
}