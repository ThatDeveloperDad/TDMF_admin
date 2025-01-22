using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using DevDad.SaaSAdmin.AccountManager.Contracts;
using DevDad.SaaSAdmin.API.ApiServices;
using DevDad.SaaSAdmin.API.PublicModels;
using DevDad.SaaSAdmin.StoreManager.Contracts;
using ThatDeveloperDad.iFX.Serialization;
using Microsoft.AspNetCore.Authorization;

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
        GuardRequiredServicesExist(app, componentRegistry, bootLogger);

        WebhookRequestCache requestCache = app.Services.GetRequiredService<WebhookRequestCache>();

        ILoggerFactory? lf = app.Services.GetRequiredService<ILoggerFactory>();
        ILogger? logger = lf.CreateLogger("WebhookEndpoints");

        RouteGroupBuilder webHookRoutes = 
            app.MapGroup("/hooks")
            .WithDescription("Endpoints that are invoked by the Webhook Receivers when a request has been succesfully authenticated.")
            .WithName("WebhookProcessors");

        webHookRoutes.MapGet("/pingTest", async (HttpContext context) =>
        {
            logger?.LogInformation("hooks/PingTest endpoint called.");
            await context.Response.WriteAsync("Hello There from WebHookReceivers!");
        });

        webHookRoutes.MapPost("/processStoreEvent"
            , async Task<IResult?>
            (InboundLsEvent lsEvent, HttpContext context) =>
            {
                IResult? operationResult = null;
                IAccountManager acctMgr = componentRegistry.GetRequiredService<IAccountManager>();

                // Disallow "test" events in non-development environments.
                // Might want to reconsider this for testing in Azure though.
                if(JsonUtilities.GetValueAtPath(lsEvent.EventJson, "$.data.test_mode") == "true"
                  && app.Environment.IsDevelopment() == false)
                {
                    logger?.LogWarning("Received a test event in a non-development environment.  Ignoring.");
                    return Results.BadRequest<string>("TestEvents are not allowed against non-development mode APIs.");
                }

                operationResult = await EndpointLogic
                    .ProcessStoreEvent(
                        storeRequestPayload: lsEvent, 
                        accountService: acctMgr, 
                        requestCache: requestCache, 
                        endpointLogger: logger);

                if(operationResult == null)
                {
                    operationResult = Results.NoContent();
                }

                return operationResult;
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
        GuardRequiredServicesExist(app, componentRegistry, bootLogger);

        ILoggerFactory? lf = app.Services.GetRequiredService<ILoggerFactory>();
        ILogger? logger = lf.CreateLogger("ApplicationEndpoints");

        app.MapGet("/pingTest", async (HttpContext context) =>
        {
            logger?.LogInformation("pingTest endpoint called.");
            await context.Response.WriteAsync("Hello There from AppEndpoints!");
        });

        app.MapPost("/loadProfile", 
        async Task<IResult> (LoadProfileRequest requestData, HttpContext httpContext) =>
        {
            IResult result = Results.NoContent();

            try
            {
                IAccountManager? acctManager = componentRegistry.GetService<IAccountManager>();
                LoadAccountProfileRequest mgrRequest = new("LoadUserProfile")
                {
                    UserId = requestData.UserEntraId
                };

                CustomerProfileResponse mgrResponse = await acctManager!.LoadCustomerProfileAsync(mgrRequest);

                if(mgrResponse.Payload == null)
                {
                    result = Results.NotFound<string?>("No profile found for the provided Id.");
                    logger.LogInformation($"No profile found for UserEntraId {requestData.UserEntraId}");
                }
                else if(mgrResponse.HasErrors)
                {
                    result = Results.BadRequest(mgrResponse.ErrorReport);
                    string errorReport = string.Join(Environment.NewLine, mgrResponse.ErrorReport);
                    logger.LogError(errorReport);
                }
                else
                {
                    result = Results.Ok(mgrResponse.Payload);
                    logger.LogInformation($"Profile loaded successfully for UserEntraId {requestData.UserEntraId}");
                }
            }
            catch(Exception ex)
            {
                logger?.LogError(ex, "An error occurred while processing the LoadProfile request.");
                result = Results.InternalServerError<string?>("An error occurred while processing your request.");
            }

            return result;
        })
        .Accepts<LoadProfileRequest>("application/json")
        .RequireAuthorization(ApiConstants.AuthorizationPolicies.AllowApiConsumersOnly);

        // This endpoint will be called from the Application when a user clicks on an
        // Upgrade to Paid Plan button.  It will send the basic information to the 
        // Store API which will create a checkout session for that user, and return
        // the URL to the checkout page at the Store Provider so that the user can
        // fill in their payment information and complete the transaction.
        app.MapPost("/startupgrade", async Task<IResult> (CreateUpgradeLinkRequest requestData, HttpContext httpContext) =>
        {
            IResult result = Results.NoContent();
            
            try
            {
                IStoreManager? storeMgr = componentRegistry.GetService<IStoreManager>();
                // If the StoreManager wasn't added tot he componentRegistry,
                // just gtfo now.
                if(storeMgr==null)
                {
                    logger.LogCritical("The StoreManager could not be instantiated to perform the operation.");
                    return Results.InternalServerError<string?>("An error occurred while processing your request.");
                }
                // convert the inbound requestData into the type expected by the
                // StoreManager.
                NewCheckoutData mgrData = new()
                {
                    LocalUserId = requestData.LocalCustomerId,
                    RequestedSku = requestData.TargetSubscriptionSku
                };

                StartCheckoutRequest mgrRequest = new("UpgradeSubscription", mgrData);
                StartCheckoutResponse? mgrResponse = await storeMgr!.StartCheckoutAsync(mgrRequest);

                if(mgrResponse == null)
                {
                    result = Results.InternalServerError<string?>("An error occurred while processing your request.");
                    logger.LogError("The StoreManager could not be instantiated to perform the operation.");
                }
                else if(mgrResponse.HasErrors)
                {
                    result = Results.BadRequest(mgrResponse.ErrorReport);
                    string errorReport = string.Join(Environment.NewLine, mgrResponse.ErrorReport);
                    logger.LogError(errorReport);
                }
                else if(mgrResponse.Successful)
                {
                    result = Results.Created(mgrResponse.CheckoutUrl, mgrResponse.CheckoutUrl);
                    logger.LogInformation($"Checkout session created successfully for WorkloadId {mgrRequest.WorkloadId}");
                }
            }
            catch(Exception ex)
            {
                logger.LogError(ex, "An error occurred while processing the StartUpgrade request.");
                result = Results.InternalServerError<string?>("An error occurred while processing your request.");
            }
            
            return result;
        });
        //TODO:  Sort out what we need to do to require Authorization on this call.
        // Once we've done that, we need to make sure all callers are set up correctly.
        // Probably require membership in a specific Entra Group.
        // Add myself as a backdoor.

        return app;
    }

    private static void GuardRequiredServicesExist(WebApplication app, IServiceProvider componentRegistry, ILogger bootLogger)
    {
        IStoreManager? storeMgr = componentRegistry.GetService<IStoreManager>();
        if(storeMgr == null)
        {
            string error = "The StoreManager service could not be loaded from appServices.  Shutting down.";
            bootLogger.LogCritical(error);
            throw new Exception(error);
        }

        IAccountManager? acctMgr = componentRegistry.GetService<IAccountManager>();
        if(acctMgr == null)
        {
            string error = "The AccountManager service could not be loaded from appServices.  Shutting down.";
            bootLogger.LogCritical(error);
            throw new Exception(error);
        }

        WebhookRequestCache? cacheTest = app.Services.GetRequiredService<WebhookRequestCache>();
        if(cacheTest == null)
        {
            string error = "The WebhookRequestCache service could not be loaded from appServices.  Shutting down.";
            bootLogger.LogCritical(error);
            throw new Exception(error);
        }
    }

}
