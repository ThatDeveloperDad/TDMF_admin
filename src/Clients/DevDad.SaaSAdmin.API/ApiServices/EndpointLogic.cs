using System;
using System.Text.Json;
using System.Threading.Tasks;
using DevDad.SaaSAdmin.AccountManager.Contracts;
using DevDad.SaaSAdmin.API.PublicModels;
using DevDad.SaaSAdmin.iFX;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ThatDeveloperDad.iFX.Serialization;

namespace DevDad.SaaSAdmin.API.ApiServices;

public class EndpointLogic
{
    public static async Task<IResult?> ProcessStoreEvent(
        InboundLsEvent storeRequestPayload,
        IAccountManager accountService,
        WebhookRequestCache requestCache,
        ILogger? endpointLogger)
    {
        IResult? operationResult = null;

        try
        {
            string subscriptionId = JsonUtilities
                .GetValueAtPath(storeRequestPayload.EventJson, "$.data.id") 
                ?? string.Empty;
            
            SubscriptionActionDetail mgrData = BuildDetailFromEventData(storeRequestPayload);

            bool eventIsProcessing = CheckEventCache(requestCache, mgrData);

            if(eventIsProcessing == true)
            {
                endpointLogger?.LogWarning($"SubscriptionId {subscriptionId} is already being processed for a {mgrData.ActionName} request.");
                return Results.Ok();
            }

            ManageSubscriptionRequest mgrRequest = new("HandleStoreEvent", mgrData);

            ManageSubscriptionResponse? mgrResponse = await accountService.ManageCustomerSubscriptionAsync(mgrRequest);

            //TODO:  Associate the logging messages with the workload that was processed.
            if(mgrResponse == null)
            {
                operationResult = Results.InternalServerError<string?>("An error occurred while processing your request.");
                endpointLogger?.LogError("The AccountManager could not be instantiated to perform the operation.");
            }
            else if(mgrResponse.HasErrors)
            {
                string errorReport = string.Join(Environment.NewLine, mgrResponse.ErrorReport);
                operationResult = Results.BadRequest(mgrResponse.ErrorReport);
                endpointLogger?.LogError(errorReport);
            }
            else if(mgrResponse.Successful)
            {
                operationResult = Results.Ok();
                endpointLogger?.LogInformation($"Subscription managed successfully for CustomerProfileId {mgrRequest.RequestDetail?.CustomerProfileId}");
            }
        }
        catch(Exception e)
        {
            endpointLogger?.LogError(e, "An error occurred while processing the Store Event.");
            operationResult = Results.InternalServerError<string?>("An error occurred while processing your request.");
        }

        return operationResult;
    }

    private static bool CheckEventCache(WebhookRequestCache cache, SubscriptionActionDetail mgrData)
    {
        bool eventInProcess;

        string subscriptionId = $"{mgrData.CustomerProfileId}-{mgrData.SubscriptionSku}";

        eventInProcess = cache.CheckActionIsProcessing(subscriptionId, mgrData.ActionName);

        return eventInProcess;
    }

    private static SubscriptionActionDetail BuildDetailFromEventData(InboundLsEvent lsEvent)
    {

        string inboundUrl = string.Empty;
        string customerIdentity = string.Empty;
        string lsCustomerId = string.Empty;
        string lsEventname = string.Empty;
        string lsTestMode = string.Empty;
        string lsSubCreatedAt = string.Empty;
        string lsSubUpdatedAt = string.Empty;
        string lsSubStatus = string.Empty;
        string lsCustomSku = string.Empty;
        string lsSubscriptionId = string.Empty;

        using(JsonDocument doc = JsonDocument.Parse(lsEvent.EventJson))
        {
            if(doc == null)
            {
                throw new InvalidOperationException("The EventJson could not be parsed.");
            }
            inboundUrl = JsonUtilities
                .GetValueAtPath(doc, "$.data.links.self")
                ?? string.Empty;
            customerIdentity = JsonUtilities
                .GetValueAtPath(doc, "$.meta.custom_data.user_identity_id")
                ?? string.Empty;
            lsSubscriptionId = JsonUtilities
                .GetValueAtPath(doc, "$.data.id")
                ?? string.Empty;
            lsCustomerId = JsonUtilities
                .GetValueAtPath(doc, "$.data.attributes.customer_id")
                ?? string.Empty;
            lsTestMode = JsonUtilities
                .GetValueAtPath(doc, "$.data.attributes.test_mode")
                ?? string.Empty;
            lsEventname = JsonUtilities
                .GetValueAtPath(doc, "$.meta.event_name")
                ?? string.Empty;
            lsSubCreatedAt = JsonUtilities
                .GetValueAtPath(doc, "$.data.attributes.created_at")
                ?? string.Empty;
            lsSubUpdatedAt = JsonUtilities
                .GetValueAtPath(doc, "$.data.attributes.updated_at")
                ?? string.Empty;
            lsSubStatus = JsonUtilities
                .GetValueAtPath(doc, "$.data.attributes.status")
                ?? string.Empty;
            lsCustomSku = JsonUtilities
                .GetValueAtPath(doc, "$.meta.custom_data.local_sku")
                ?? string.Empty;
        }

        if(inboundUrl.Contains(ExternalServiceVendors.LemonSqueezy, StringComparison.OrdinalIgnoreCase) == false)
        {
            throw new InvalidOperationException("The EventJson did not originate from LemonSqueezy.");
        }
        
        string localActionName = ChooseAction(lsEventname, lsSubCreatedAt, lsSubUpdatedAt, lsSubStatus);

        SubscriptionActionDetail detail = new()
        {
            CustomerProfileId = customerIdentity,
            RequestSource = ChangeRequestSource.VendorWebHook,
            VendorName = ExternalServiceVendors.LemonSqueezy,
            VendorSuppliedCustomerId = lsCustomerId,
            ActionName = localActionName,
            SubscriptionSku = lsCustomSku
        };

        return detail;
    }

    private static string ChooseAction(
        string lsEventname,
        string lsSubCreatedAt,
        string lsSubUpdatedAt,
        string lsSubStatus)
    {
        string selectedAction = string.Empty;   
        DateTime? createdDate = null;
        DateTime? updatedDate = null;

        string lsActiveStatus = "active";

        if(DateTime.TryParse(lsSubCreatedAt, out DateTime parsedCreate))
        {
            createdDate = parsedCreate.Date;
        }
        if(DateTime.TryParse(lsSubUpdatedAt, out DateTime parsedUpdate))
        {
            updatedDate = parsedUpdate.Date;
        }
        
        bool isActive = lsSubStatus == lsActiveStatus;

        switch(lsEventname)
        {
            case "subscription_created":
                selectedAction = SubscriptionChangeKinds.ActivityKind_Create;
                break;

            case "subscription_updated":
                // This covers more than one kind of change.
                if(isActive)
                {
                    if(createdDate == updatedDate)
                    {
                        selectedAction = SubscriptionChangeKinds.ActivityKind_Create;
                    }
                    else if(createdDate <= DateTime.Today.AddDays(-25))
                    {
                        selectedAction = SubscriptionChangeKinds.ActivityKind_Renew;
                    }
                }
                else
                {
                    selectedAction = SubscriptionChangeKinds.ActivityKind_Expire;
                }
                
                break;

            case "subscription_cancelled":
            case "subscription_payment_refunded":
                selectedAction = SubscriptionChangeKinds.ActivityKind_Cancel;
                break;

            case "subscription_expired":             
                selectedAction = SubscriptionChangeKinds.ActivityKind_Expire;
                break;
            
            default:
                break;
        }


        return selectedAction;
    }
}
