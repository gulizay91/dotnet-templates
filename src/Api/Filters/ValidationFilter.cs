using Api.Filters.ValidationModels;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Api.Filters;

public class ValidationFilterAttribute : ActionFilterAttribute
{
  public override void OnActionExecuting(ActionExecutingContext context)
  {
    if (!context.ModelState.IsValid) context.Result = new ValidationFailedResult(context.ModelState);
  }
}