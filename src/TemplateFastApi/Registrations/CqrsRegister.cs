using System.Reflection;
using MediatR;
using FluentValidation;
using TemplateFastApi.Application.Behaviours;

namespace TemplateFastApi.Registrations;

public static class CqrsRegister
{
  public static void RegisterMediatr(this IServiceCollection serviceCollection)
  {
    serviceCollection.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
    serviceCollection.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
    serviceCollection.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
  }
}