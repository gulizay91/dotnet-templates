using FastEndpoints.Swagger;

namespace TemplateFastApi.Registrations;

public static class SwaggerRegister
{
  public static void RegisterSwagger(this IServiceCollection serviceCollection)
  {
    var versions = GetApiVersions();
    foreach (var version in versions)
    {
      serviceCollection.SwaggerDocument(o =>
      {
        o.MaxEndpointVersion = version;
        o.TagCase = TagCase.LowerCase;
        o.DocumentSettings = s =>
        {
          s.DocumentName = $"v{version}";
          s.Title = $"Template FastAPI v{version}";
          s.Version = $"v{version}";
        };
      });
    }
  }

  public static void UseSwagger(this IApplicationBuilder applicationBuilder)
  {
    applicationBuilder.UseSwaggerGen();
    // Configure Swagger UI to display each version dynamically
    applicationBuilder.UseSwaggerUI(c =>
    {
      var versions = GetApiVersions();
      foreach (var version in versions)
      {
        // Add Swagger endpoints dynamically for each version
        c.SwaggerEndpoint($"/swagger/v{version}/swagger.json", $"Template FastAPI v{version}");
      }
      
      c.DefaultModelsExpandDepth(-1); // Collapses the "Schemas" section by default
      c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None); // Collapses all endpoints initially
      c.DisplayRequestDuration(); // Displays the duration of each request in Swagger UI
    });
  }
  
  private static IEnumerable<int> GetApiVersions()
  {

    return [1, 2];
  }
}