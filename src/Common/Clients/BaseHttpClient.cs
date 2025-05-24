using System.Net.Http.Json;
using System.Text.Json;

namespace Common.Clients;

public class BaseHttpClient : IBaseHttpClient
{
  private readonly HttpClient _httpClient;

  protected BaseHttpClient(HttpClient httpClient)
  {
    _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
  }
  
  public async Task<T?> GetAsync<T>(HttpRequestMessage requestMessage)
  {
    var response = await _httpClient.SendAsync(requestMessage);
    response.EnsureSuccessStatusCode();

    var responseContent = await response.Content.ReadAsStringAsync();
    var options = new JsonSerializerOptions
    {
      PropertyNameCaseInsensitive = true
    };
    return JsonSerializer.Deserialize<T>(responseContent, options);
  }
  
  public async Task<T?> PostAsync<T, TR>(HttpRequestMessage requestMessage, TR content)
  {
    requestMessage.Content = JsonContent.Create(content);

    var response = await _httpClient.SendAsync(requestMessage);
    response.EnsureSuccessStatusCode();

    await using var contentStream = await response.Content.ReadAsStreamAsync();
    return await JsonSerializer.DeserializeAsync<T>(contentStream);
  }
}