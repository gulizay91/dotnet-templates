using System.Text.Json.Serialization;
using Common.Exchanges.Response;

namespace TemplateFastApi.Application.Exchanges.Response;

public class GetTemplatesResponse : ServiceResponse<string>
{
  [JsonConstructor]
  public GetTemplatesResponse(string? templateId, bool success, int statusCode, string? message = null, string? errorCode = null, List<ErrorDetail>? errors = null)
    : base(success, statusCode, message, errorCode, templateId, errors)
  {
  }
}