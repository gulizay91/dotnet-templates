using Api.Settings;
using Microsoft.Extensions.Options;

namespace Api.Filters.AuthenticationModels;

public class ApiKeyValidator : IApiKeyValidator
{
  private readonly IOptions<ClientSettings> _clientSettings;

  public ApiKeyValidator(IOptions<ClientSettings> clientSettings)
  {
    _clientSettings = clientSettings;
  }

  public bool IsMandatory()
  {
    return _clientSettings.Value.ApiClient.AuthorizationEnabled;
  }

  public bool IsValid(string apiKey)
  {
    // Implement logic for validating the API key.
    return _clientSettings.Value.ApiClient.ApiKey.Equals(apiKey);
  }
  
  private Client? GetClient(string clientIdentity)
  {
    if (_clientSettings.Value.ApiClient.Url.Contains(clientIdentity))
      return _clientSettings.Value.ApiClient;
    if (_clientSettings.Value.SampleApiClient.Url.Contains(clientIdentity))
      return _clientSettings.Value.SampleApiClient;
    return null;
  }
}

public interface IApiKeyValidator
{
  bool IsValid(string apiKey);
  bool IsMandatory();
}