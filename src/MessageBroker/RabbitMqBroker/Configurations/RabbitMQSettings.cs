using System.ComponentModel.DataAnnotations;

namespace MessageBroker.RabbitMqBroker.Configurations;

public record RabbitMQSettings
{
  [Required(ErrorMessage = "Hostname is required")]
  public required string Hostname { get; init; }
  
  [Required(ErrorMessage = "Username is required")]
  public required string Username { get; init; }
  
  [Required(ErrorMessage = "Password is required")]
  public required string Password { get; init; }
  
  public Dictionary<string, ConsumerSettings>? Consumers { get; set; }
  public Dictionary<string, PublisherSettings>? Publishers { get; set; }
}