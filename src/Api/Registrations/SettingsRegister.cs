using Api.Settings;
using Api.Settings.Validators;
using Microsoft.Extensions.Options;

namespace Api.Registrations;

public static class SettingsRegister
{
  public static void RegisterSettings(this IServiceCollection serviceCollection, IConfiguration configuration)
  {
    serviceCollection.AddOptions<ClientSettings>().ValidateOnStart();
    
    serviceCollection.Configure<ClientSettings>(options =>
    {
      configuration.GetSection(nameof(ClientSettings)).Bind(options);

      options.AddApiClientSettings(configuration);
      options.AddSampleApiClientSettings(configuration);
    });
    
    serviceCollection.AddSingleton<IValidateOptions<ClientSettings>, ClientSettingsValidation>();
  }
  
  private static void AddApiClientSettings(this ClientSettings options, IConfiguration configuration)
  {
    options.ApiClient = configuration
      .GetSection($"{nameof(ClientSettings)}:{nameof(ApiClient)}")
      .Get<ApiClient>();
  }
  
  private static void AddSampleApiClientSettings(this ClientSettings options, IConfiguration configuration)
  {
    options.SampleApiClient = configuration
      .GetSection($"{nameof(ClientSettings)}:{nameof(SampleApiClient)}")
      .Get<SampleApiClient>();
  }
}