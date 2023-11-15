namespace Api.Settings;

public record ClientSettings
{
  public ApiClient ApiClient { get; set; }
  public SampleApiClient SampleApiClient { get; set; }
}

public record ApiClient : Client;

public record SampleApiClient : Client;


public abstract record Client : AuthorizationSettings
{
  public string Url { get; set; }
}

public abstract record AuthorizationSettings
{
  public string ApiKey { get; set; }
  public bool AuthorizationEnabled { get; set; } = false;
}