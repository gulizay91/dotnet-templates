namespace Common.Contracts.Commands;

public record CreateTemplateCommand : ICommand
{
  public string TemplateId { get; init; }

  public CreateTemplateCommand( string templateId)
  {
    TemplateId = templateId;
  }

  public Guid CorrelationId { get; set; }
}