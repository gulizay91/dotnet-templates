using WorkerConsumer.Registrations;

var builder = Host.CreateApplicationBuilder(args);

Console.WriteLine($"ASPNETCORE_ENVIRONMENT: {Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}");

builder.RegisterConfiguration();

builder.Services.RegisterHealthChecks();
builder.Services.RegisterLoggers(builder.Configuration);
builder.Services.RegisterSettings(builder.Configuration);
builder.Services.RegisterHttpClients(builder.Configuration);
builder.Services.RegisterServices();
builder.Services.RegisterConsumer(builder.Configuration);

var host = builder.Build();
await host.RunAsync();