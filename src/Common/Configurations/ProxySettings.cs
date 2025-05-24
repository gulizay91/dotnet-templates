using System.ComponentModel.DataAnnotations;

namespace Common.Configurations;

public record ProxySettings
{
  [Required(ErrorMessage = "Url is required")]
  public string Url { get; init; }

  [Required(ErrorMessage = "Timeout is required")]
  public int Timeout { get; init; } = 3;

  [Required(ErrorMessage = "RetryAttempts is required")]
  public int RetryAttempts { get; init; } = 5;
}