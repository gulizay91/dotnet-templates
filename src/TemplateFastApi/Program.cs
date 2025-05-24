using FastEndpoints;
using MessageBroker.RabbitMqBroker.Registrations;
using TemplateFastApi.Middlewares;
using TemplateFastApi.Registrations;

var builder = WebApplication.CreateBuilder(args);
Console.WriteLine($"ASPNETCORE_ENVIRONMENT: {Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}");
builder.Services.AddFastEndpoints();
builder.Services.RegisterHealthChecks();

builder.Services.RegisterLoggers(builder.Configuration);
builder.Services.RegisterMediatr();

builder.Services.RegisterRabbitMQ(builder.Configuration);
builder.Services.RegisterServices();
builder.Services.RegisterSwagger();

var app = builder.Build();

app.UseGlobalErrorHandler();
app.UseRouting();
app.UseSwagger();
app.UseFastEndpoints(c =>
{
  c.Endpoints.RoutePrefix = "api";
  c.Versioning.Prefix = "v";
  c.Versioning.PrependToRoute = true;
});
app.UseHealthCheckEndpoints();

app.Run();