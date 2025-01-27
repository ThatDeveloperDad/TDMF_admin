using DevDad.SaaSAdmin.Functions.LocalServices;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = FunctionsApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

var apiOptions = new AdminApiOptions();
builder.Configuration.GetSection("AdminApi").Bind(apiOptions);

builder.Services.AddSingleton(apiOptions);
builder.ConfigureFunctionsWebApplication();
builder.Services.AddLogging();
builder.Logging.AddConsole();

builder.Services.AddHttpClient();

builder.Build().Run();
