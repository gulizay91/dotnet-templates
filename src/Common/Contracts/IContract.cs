namespace Common.Contracts;

public interface IContract
{
  public Guid CorrelationId { get; set; }
}