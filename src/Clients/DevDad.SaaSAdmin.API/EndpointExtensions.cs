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

        // This endpoint will be called from the Application when a user clicks on an
        // Upgrade to Paid Plan button.  It will send the basic information to the 
        // Store API which will create a checkout session for that user, and return
        // the URL to the checkout page at the Store Provider so that the user can
        // fill in their payment information and complete the transaction.
        app.MapPost("/startupgrade", (CreateUpgradeLinkRequest requestData, HttpContext httpContext) =>
        {
            // Need a LemonSqueezy API Provider.
            // Need to put that API Provider behind a Store Manager.
            // For now, the manager will have one method:  
            // StartCheckout, and it will receive a
            // CreateUpgradeLinkRequest.

            return httpContext.Response.WriteAsync("General Kenobi, I've been expecting you!");
        });
        
        

        return app;
    }

}
