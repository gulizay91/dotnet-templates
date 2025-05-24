namespace WorkerConsumer.Consumers;

public interface IConsumer<in TMessage> where TMessage : class
{
  Task<bool> ConsumeAsync(TMessage message, CancellationToken cancellationToken);
}