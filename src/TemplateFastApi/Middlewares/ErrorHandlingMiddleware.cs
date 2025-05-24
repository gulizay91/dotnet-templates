using System.Net;
using FluentValidation;
using Common.Exchanges.Response;
using Microsoft.AspNetCore.Diagnostics;

namespace TemplateFastApi.Middlewares;

public static class ErrorHandlingMiddleware
{
  public static void UseGlobalErrorHandler(this IApplicationBuilder app)
  {
    app.UseExceptionHandler(errorApp =>
    {
      errorApp.Run(async context =>
      {
        context.Response.ContentType = "application/json";

        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
        if (exceptionHandlerPathFeature?.Error == null)
          return;

        var exception = exceptionHandlerPathFeature.Error;
        var statusCode = exception switch
        {
          ValidationException => (int)HttpStatusCode.BadRequest,
          _ => (int)HttpStatusCode.InternalServerError
        };

        BaseResponse errorResponse;

        if (exception is ValidationException validationException)
        {
          var errorDetails = validationException.Errors
            .Select(e => new ErrorDetail
            {
              PropertyName = e.PropertyName,
              ErrorMessage = e.ErrorMessage,
              AttemptedValue = e.AttemptedValue
            })
            .ToList();

          errorResponse = BaseResponse.ErrorResponse("Validation failed!", statusCode, errorDetails);
        }
        else
        {
          errorResponse = BaseResponse.ErrorResponse(exception.Message, statusCode);
        }

        context.Response.StatusCode = statusCode;
        await context.Response.WriteAsJsonAsync(errorResponse);
      });
    });
  }
}