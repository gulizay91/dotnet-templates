using Api.Constans;
using Api.Exceptions;
using Api.Filters.AuthenticationModels;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Api.Filters;

// ref: https://www.milanjovanovic.tech/blog/how-to-implement-api-key-authentication-in-aspnet-core
public class ApiKeyAuthorizationFilter : IAuthorizationFilter
{
  private const string ApiKeyHeaderName = ApiKeyAuthenticationDefaults.AuthenticationScheme;
  private readonly IApiKeyValidator _apiKeyValidator;

  public ApiKeyAuthorizationFilter(IApiKeyValidator apiKeyValidator)
  {
    _apiKeyValidator = apiKeyValidator;
  }

  public void OnAuthorization(AuthorizationFilterContext context)
  {
    string? apiKey = context.HttpContext.Request.Headers[ApiKeyHeaderName];
    // ps: if we dont wanna use middleware, uncomment
    // if (_apiKeyValidator.IsMandatory() && (_apiKeyValidator.string.IsNullOrWhiteSpace(apiKey) || !_apiKeyValidator.IsValid(apiKey)))
    // {
    //     context.Result = new UnauthorizedResult();
    // }
    if (!_apiKeyValidator.IsMandatory()) return;
    if (string.IsNullOrWhiteSpace(apiKey)) throw new UnauthorizedException("API Key is missing!");
    if (!_apiKeyValidator.IsValid(apiKey)) throw new UnauthorizedException("Invalid API Key!");
  }
}