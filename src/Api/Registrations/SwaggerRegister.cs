using System.Reflection;
using Api.Constans;
using Api.Filters;
using Microsoft.OpenApi.Models;


namespace Api.Registrations;

public static class SwaggerRegister
{
  public static void RegisterSwagger(this IServiceCollection serviceCollection)
  {
    serviceCollection.AddEndpointsApiExplorer();

    serviceCollection.AddSwaggerGen(options =>
    {
      var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
      var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
      options.IncludeXmlComments(xmlPath);
      options.EnableAnnotations();
      options.SchemaFilter<SamplePutSchemaFilter>();
      options.AddSecurityDefinition(ApiKeyAuthenticationDefaults.AuthenticationScheme,
        new OpenApiSecurityScheme
        {
          Description = "API key used in the Authorization header.",
          Name = ApiKeyAuthenticationDefaults.AuthenticationScheme,
          In = ParameterLocation.Header,
          Type = SecuritySchemeType.ApiKey
        });
      options.AddSecurityRequirement(new OpenApiSecurityRequirement
      {
        {
          new OpenApiSecurityScheme
          {
            Reference = new OpenApiReference
            {
              Type = ReferenceType.SecurityScheme,
              Id = ApiKeyAuthenticationDefaults.AuthenticationScheme
            }
          },
          Array.Empty<string>()
        }
      });
    });
  }
}