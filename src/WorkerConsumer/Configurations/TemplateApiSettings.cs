using System.ComponentModel.DataAnnotations;
using Common.Configurations;

namespace WorkerConsumer.Configurations;

public record TemplateApiSettings
{
  [Required(ErrorMessage = "ProxySettings is required")]
  public required ProxySettings ProxySettings { get; init; }
}