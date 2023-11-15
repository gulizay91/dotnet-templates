using System.Text.Json.Serialization;

namespace Api.Filters.ValidationModels;

public class ValidationError
{
  public ValidationError(string field, string message)
  {
    Field = (field != string.Empty ? field : null)!;
    Message = message;
  }

  [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
  public string Field { get; }

  public string Message { get; }
}