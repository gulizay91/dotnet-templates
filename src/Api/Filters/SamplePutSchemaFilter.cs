using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using SystemTextJsonPatch;

namespace Api.Filters;

public class SamplePutSchemaFilter: ISchemaFilter
{
  public void Apply(OpenApiSchema schema, SchemaFilterContext context)
  {
    if (context.Type == typeof(JsonPatchDocument<object>))
      schema.Example = new OpenApiArray
      {
        new OpenApiObject
        {
          ["op"] = new OpenApiString("replace"),
          ["path"] = new OpenApiString("/status"),
          ["value"] = new OpenApiString("value")
        }
      };
  }
}