
using System;
using DevDad.SaaSAdmin.API.ApiServices;
using DotNetEnv;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ThatDeveloperDad.iFX;

namespace DevDad.SaaSAdmin.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddOpenApi();
        var bootLogger = CreateBootLogger();
		IConfiguration systemConfig = LoadSystemConfiguration(bootLogger);
		builder = AddUtilityServices(systemConfig, bootLogger, builder);	

        // Add services to the container.
        builder.Services.AddAuthorization(options => 
        {
            // Need to check the app Environment to determine if we're in Dev or Prod here.
            var environment = builder.Environment;

            if (environment.IsDevelopment())
            {
                options.AddPolicy(ApiConstants.AuthorizationPolicies.AllowApiConsumersOnly, 
                    policy => policy.RequireAssertion(_ => true));
            }
            else
            {
                options.AddPolicy(ApiConstants.AuthorizationPolicies.AllowApiConsumersOnly, 
                    policy => policy.RequireAuthenticatedUser());
            }
        });

        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi


        var app = builder.Build();

        app.UseHttpsRedirection();

        app.UseAuthorization();

        // All the services we'd registered in the Application's DI container are considered
        // "Ambient" or "Global" services, and really should just consist of Utility style things.
        // our APPLICATION components will use a different DI container, that we will set up now.
        
        // The globalUtilities variable is the app-level DI Container.  Let's get that into a variable now.
        var globalUtilities = app.Services;
        IServiceCollection appServicesBuilder = new ServiceCollection();
			appServicesBuilder = appServicesBuilder.AddAppArchitecture(
				systemConfig,
				globalUtilities,
				bootLogger);

        // ASP.Net Core's framework REALLY hates that I'm using 2 DI Containers
        // but too bad.
        // The pragma warnign disables this instance of the Compiler warning
        // because I'm doing this on purpose to isolate my logic component tree
        // from the rest of the application's ambient services.
#pragma warning disable ASP0000 // Do not call 'IServiceCollection.BuildServiceProvider' in 'ConfigureServices'
        IServiceProvider appServices = appServicesBuilder.BuildServiceProvider();
#pragma warning restore ASP0000 // Do not call 'IServiceCollection.BuildServiceProvider' in 'ConfigureServices'

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();    
        }

        // These next methods are defined in EndpointExtensions.cs
        bootLogger.LogInformation("Configuring API Endpoints.");
        app.AddWebhookEndpoints(appServices, bootLogger);
        app.AddApplicationEndpoints(appServices, bootLogger);

        app.Run();
    }


    static WebApplicationBuilder AddUtilityServices(IConfiguration systemConfig,
			ILogger bootLog,
            WebApplicationBuilder appBuilder)
    {
        bootLog.LogInformation("Configuring Utility Provider");
        IServiceCollection serviceBuilder = appBuilder.Services;
        
        serviceBuilder = ConfigureLogging(serviceBuilder, systemConfig, bootLog);

        // When we need to add things like an IHttpClientFactory, and other
        // non-domain-specific services to the utility provider, we'll do that
        // here.
        serviceBuilder.AddHttpClient();

        serviceBuilder.AddMemoryCache();
        serviceBuilder.AddSingleton<WebhookRequestCache>();
        
        return appBuilder;
    }

    private static IServiceCollection ConfigureLogging(
        IServiceCollection serviceBuilder,
        IConfiguration config,
        ILogger? logger = null)
    {
        try
        {
            serviceBuilder.AddLogging(logBuilder =>
            {
                var logConfig = config.GetSection("Logging");
                if(logConfig != null)
                {
                    logBuilder.AddConfiguration(logConfig);
                }
                logBuilder.AddConsole();
            });
            logger?.LogInformation("Global Logging Added to SharedServices.");
        }
        catch (Exception ex)
        {
            logger?.LogWarning(ex, "Global logging could not be added.  System will not log at runtime.");
        }

        return serviceBuilder;
    }

    private static ILogger CreateBootLogger()
    {
        ILoggerFactory loggerFactory = LoggerFactory.Create(
            builder =>
            {
                builder.AddConsole();
            }
        );

        ILogger logger = loggerFactory.CreateLogger(nameof(Program));

        logger.LogInformation("App BootLogger Created.");
        return logger;
    }
    
    private static IConfiguration LoadSystemConfiguration(ILogger bootLog)
    {
        #if DEBUG
        bootLog.LogInformation("Running in local debug mode.  Load custom environment variables from .env file.");
        Env.Load();
        #endif
        
        var builder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables();
        
        bootLog.LogInformation("Configuration Loaded.");

        return builder.Build();
    }
}