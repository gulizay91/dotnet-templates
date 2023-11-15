using System.Text.Json.Serialization;
using Api.Filters;
using Api.Filters.AuthenticationModels;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;

namespace Api.Registrations;

public static class ControllerRegister
{
  public static void RegisterControllers(this IServiceCollection serviceCollection)
  {
    serviceCollection.AddControllers().AddJsonOptions(options =>
    {
      options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
    serviceCollection.AddFluentValidationAutoValidation();
    serviceCollection.AddValidatorsFromAssembly(typeof(Program).Assembly);
    serviceCollection.Configure<ApiBehaviorOptions>(options => { options.SuppressModelStateInvalidFilter = true; });
    serviceCollection.AddSingleton<ApiKeyAuthorizationFilter>();
    serviceCollection.AddSingleton<IApiKeyValidator, ApiKeyValidator>();
    
    

  }
}