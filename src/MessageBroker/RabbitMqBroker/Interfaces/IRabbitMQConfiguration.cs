using MessageBroker.RabbitMqBroker.Configurations;

namespace MessageBroker.RabbitMqBroker.Interfaces;

public interface IRabbitMQConfiguration
{
  Dictionary<string, ConsumerSettings> Consumers { get; }
  Dictionary<string, PublisherSettings> Publishers { get; }
  string GetConsumerQueueName(string consumerKey);
  string GetConsumerExchangeName(string consumerKey);
  string GetConsumerExchangeType(string consumerKey);
  int GetConsumerRetryMaxCount(string consumerKey);
  int GetConsumerRetryDelaySeconds(string consumerKey);
  bool GetConsumerHasDlq(string consumerKey);
  bool GetConsumerIsDurable(string consumerKey);

  string? GetPublisherExchangeName(string publisherKey);
  string? GetPublisherQueueName(string publisherKey);
}