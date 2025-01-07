using System;
using System.Linq;
using System.Threading.Tasks;
using DevDad.SaaSAdmin.AccountManager.Contracts;
using DevDad.SaaSAdmin.API.PublicModels;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DevDad.SaaSAdmin.API;

public static class EndpointExtensions
{

    /// <summary>
    /// These Api Endpoints are called by the Webhook Recievers after the event arrives
    /// and the WebHook invocation has been validated as coming from a trusted source.
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static WebApplication? AddWebhookEndpoints(this WebApplication app, 
        IServiceProvider componentRegistry,
        ILogger bootLogger)
    {
        IAccountManager? useCaseProvider = componentRegistry.GetService<IAccountManager>();
        if(useCaseProvider == null)
        {
            string error = "The IAccountManager service could not be loaded from appServices.  Shutting down.";
            bootLogger.LogError("The IAccountManager service could not be loaded from appServices.  Shutting down.");
            throw new InvalidOperationException(error);
        }

        RouteGroupBuilder webHookRoutes = 
            app.MapGroup("/hooks")
            .WithDescription("Endpoints that are invoked by the Webhook Receivers when a request has been succesfully authenticated.")
            .WithName("WebhookProcessors");

        webHookRoutes.MapPost("/processStoreEvent"
            , async Task<HttpContext>
            (InboundLsEvent lsEvent, HttpContext context) =>
            {
                await context.Response.WriteAsync("Processed New Entra Signup!");
                return context;
            })
            .WithName("ProcessStoreEvent");

        webHookRoutes.MapPost("/processNewEntraSignup", async (HttpContext context) =>
        {
            await context.Response.WriteAsync("Processed New Entra Signup!");
        })
        .WithName("ProcessNewEntraSignup");

        return app;
    }

    /// <summary>
    /// These Api Endpoints are called by the Product Application or the Admin Application
    /// as needed.
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static WebApplication? AddApplicationEndpoints(this WebApplication app, 
        IServiceProvider componentRegistry,
        ILogger bootLogger)
    {

        app.MapGet("/helloThere", (HttpContext httpContext) =>
        {
            return httpContext.Response.WriteAsync("General Kenobi, I've been expecting you!");
        })
        .WithName("Greeting");
        
        var summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        app.MapGet("/weatherforecast", (HttpContext httpContext) =>
        {
            /* var forecast =  Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                {
                    Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    TemperatureC = Random.Shared.Next(-20, 55),
                    Summary = summaries[Random.Shared.Next(summaries.Length)]
                })
                .ToArray(); */
            return "It's winter in North Carolina.  Enjoy your milk sandwiches!";
        })
        .WithName("TommyCanYouSeeMe");

        return app;
    }

}
