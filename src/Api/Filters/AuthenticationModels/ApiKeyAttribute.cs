using Microsoft.AspNetCore.Mvc;

namespace Api.Filters.AuthenticationModels;

public class ApiKeyAttribute : ServiceFilterAttribute
{
  public ApiKeyAttribute()
    : base(typeof(ApiKeyAuthorizationFilter))
  {
  }
}