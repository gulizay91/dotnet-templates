using MessageBroker.RabbitMqBroker.Configurations;
using MessageBroker.RabbitMqBroker.Interfaces;
using Microsoft.Extensions.Options;

namespace MessageBroker.RabbitMqBroker.Implementations;

public class RabbitMQConfiguration : IRabbitMQConfiguration
{
  private readonly RabbitMQSettings _rabbitMQSettings;

  public RabbitMQConfiguration(IOptions<RabbitMQSettings> options)
  {
    _rabbitMQSettings = options.Value;
  }

  private ConsumerSettings GetConsumerSettings(string consumerKey)
  {
    if (_rabbitMQSettings.Consumers is null || !_rabbitMQSettings.Consumers.TryGetValue(consumerKey, out var settings))
    {
      throw new ArgumentException($"Consumer key '{consumerKey}' not found.");
    }
    

    return settings;
  }
  
  private PublisherSettings GetPublisherSettings(string publisherKey)
  {
    if (_rabbitMQSettings.Publishers is null || !_rabbitMQSettings.Publishers.TryGetValue(publisherKey, out var settings))
    {
      throw new ArgumentException($"Publisher key '{publisherKey}' not found.");
    }

    return settings;
  }

  public Dictionary<string, ConsumerSettings> Consumers => _rabbitMQSettings.Consumers ?? new();
  public Dictionary<string, PublisherSettings> Publishers => _rabbitMQSettings.Publishers ?? new();
  public string GetConsumerQueueName(string consumerKey) => GetConsumerSettings(consumerKey).QueueName;
  public string GetConsumerExchangeName(string consumerKey) => GetConsumerSettings(consumerKey).ExchangeName;
  public string GetConsumerExchangeType(string consumerKey) => GetConsumerSettings(consumerKey).ExchangeType;
  public int GetConsumerRetryMaxCount(string consumerKey) => GetConsumerSettings(consumerKey).RetryMaxCount;
  public int GetConsumerRetryDelaySeconds(string consumerKey) => GetConsumerSettings(consumerKey).RetryDelaySecond;
  public bool GetConsumerHasDlq(string consumerKey) => GetConsumerSettings(consumerKey).HasDlq;
  public bool GetConsumerIsDurable(string consumerKey) => GetConsumerSettings(consumerKey).Durable;
  
  public string? GetPublisherExchangeName(string publisherKey) => GetPublisherSettings(publisherKey).ExchangeName;
  public string? GetPublisherQueueName(string publisherKey) => GetPublisherSettings(publisherKey).QueueName;
}