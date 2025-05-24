using System.Net;
using System.Text.Json;
using Common.Contracts.Commands;
using Common.Loggers;
using MessageBroker.RabbitMqBroker.Interfaces;
using TemplateFastApi.Application.Contracts;
using TemplateFastApi.Application.Contracts.Commands;
using TemplateFastApi.Application.Exchanges.Response;

namespace TemplateFastApi.Application.Handlers;

public class SendTemplateCommandHandler : IRequestHandlerWrapper<SendTemplateCommand, SendTemplateResponse>
{
  private readonly IRabbitMQClient _rabbitMQClient;
  private readonly IRabbitMQConfiguration _config;
  private readonly IConsoleLogger _logger;

  public SendTemplateCommandHandler(IRabbitMQClient rabbitMQClient, IRabbitMQConfiguration config, IConsoleLogger logger)
  {
    _rabbitMQClient = rabbitMQClient;
    _config = config;
    _logger = logger;
  }

  public async Task<SendTemplateResponse> Handle(SendTemplateCommand request, CancellationToken cancellationToken)
  {
    try
    {
      ArgumentException.ThrowIfNullOrWhiteSpace(_config.GetPublisherExchangeName("TemplatePublisher"));

      var createTemplateCommand = new CreateTemplateCommand(request.TemplateId.ToString())
      {
        CorrelationId = Guid.NewGuid()
      };
      
      await _rabbitMQClient.PublishMessageAsync(_config.GetPublisherExchangeName("TemplatePublisher")!,
        JsonSerializer.Serialize(createTemplateCommand), cancellationToken: cancellationToken);
    }
    catch (Exception ex)
    {
      await _logger.LogError("Error publish template message.", exception: ex, className: nameof(SendTemplateCommandHandler), methodName: nameof(Handle));
      return new SendTemplateResponse(false, (int)HttpStatusCode.InternalServerError, "Failed publish template message");
    }
    return new SendTemplateResponse(true, (int)HttpStatusCode.OK, "Send template");
  }
}