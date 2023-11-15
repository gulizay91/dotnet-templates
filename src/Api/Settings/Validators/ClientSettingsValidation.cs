using System.Text.Json;
using Microsoft.Extensions.Options;

namespace Api.Settings.Validators;

public class ClientSettingsValidation: IValidateOptions<ClientSettings>
{
    private readonly ILogger<ClientSettingsValidation> _logger;

    public ClientSettingsValidation(ILogger<ClientSettingsValidation> logger)
    {
        _logger = logger;
    }

    public ValidateOptionsResult Validate(string name, ClientSettings options)
    {
        _logger.LogTrace($"{nameof(ClientSettings)}:{JsonSerializer.Serialize(options)}");

        var resultApiClient = ValidateOptionsForApiClient(options);
        if (resultApiClient is not null) return resultApiClient;

        var resultSampleApiClient = ValidateOptionsForSampleApiClient(options);
        if (resultSampleApiClient is not null) return resultSampleApiClient;

        return ValidateOptionsResult.Success;
    }

    private ValidateOptionsResult? ValidateOptionsForApiClient(ClientSettings options)
    {
        ArgumentNullException.ThrowIfNull(options.ApiClient);

        if (string.IsNullOrWhiteSpace(options.ApiClient.Url))
        {
            _logger.LogError(
                $"{options.GetType().Name}:{nameof(ApiClient)}:{nameof(options.ApiClient.Url)} is null");
            return ValidateOptionsResult.Fail(
                $"{options.GetType().Name}:{nameof(ApiClient)}:{nameof(options.ApiClient.Url)} is null");
        }
        
        if (string.IsNullOrWhiteSpace(options.ApiClient.ApiKey))
        {
            _logger.LogError(
                $"{options.GetType().Name}:{nameof(ApiClient)}:{nameof(options.ApiClient.ApiKey)} is null");
            return ValidateOptionsResult.Fail(
                $"{options.GetType().Name}:{nameof(ApiClient)}:{nameof(options.ApiClient.ApiKey)} is null");
        }

        return null;
    }
    
    private ValidateOptionsResult? ValidateOptionsForSampleApiClient(ClientSettings options)
    {
        ArgumentNullException.ThrowIfNull(options.SampleApiClient);
        if (string.IsNullOrWhiteSpace(options.SampleApiClient.Url))
        {
            _logger.LogError(
                $"{options.GetType().Name}:{nameof(ApiClient)}:{nameof(options.SampleApiClient.Url)} is null");
            return ValidateOptionsResult.Fail(
                $"{options.GetType().Name}:{nameof(ApiClient)}:{nameof(options.SampleApiClient.Url)} is null");
        }

        return null;
    }
}