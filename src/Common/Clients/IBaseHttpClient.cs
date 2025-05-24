namespace Common.Clients;

public interface IBaseHttpClient
{
  Task<T?> PostAsync<T, TR>(HttpRequestMessage requestMessage, TR content);

  Task<T?> GetAsync<T>(HttpRequestMessage requestMessage);
}