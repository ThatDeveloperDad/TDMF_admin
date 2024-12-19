using System;
using System.Collections.Generic;
using System.Linq;
using DevDad.SaaSAdmin.AccountManager.Contracts;
using DevDad.SaaSAdmin.iFX;
using ThatDeveloperDad.iFX.ObjectUtilities;
using ThatDeveloperDad.iFX.ServiceModel;

namespace DevDad.SaaSAdmin.AccountManager.Internals.SubscriptionStrategies;

/// <summary>
/// Validates the REQUEST object for a Change.
/// </summary>
internal class ChangeRequestValidator : IValidator<ChangeStrategyRequest?>
{
    public IEnumerable<ServiceError> Validate(ChangeStrategyRequest? instance)
    {
        List<ServiceError> errors = new();

        if (instance == null)
        {
            ServiceError error = new()
            {
                Message = "ChangeStrategyRequest instance is null.",
                Severity = ErrorSeverity.Error,
                ErrorKind = ServiceErrorKinds.RequestValidation
            };

            errors.Add(error);
            // instance null.  Nothing else makes sense.  Return early.
            return errors;
        }

        if(instance.ChangeDetails == null)
        {
            ServiceError error = new()
            {
                Message = "ChangeStrategyRequest.ChangeDetails is null.",
                Severity = ErrorSeverity.Error,
                ErrorKind = ServiceErrorKinds.RequestValidation
            };

            errors.Add(error);
        }

        if(instance.TargetTemplate == null)
        {
            ServiceError error = new()
            {
                Message = "ChangeStrategyRequest.TargetTemplate is null.",
                Severity = ErrorSeverity.Error,
                ErrorKind = ServiceErrorKinds.RequestValidation
            };

            errors.Add(error);
        }

        if(instance.SubscriptionToUpdate == null)
        {
            ServiceError error = new()
            {
                Message = "ChangeStrategyRequest.SubscriptionToUpdate is null.  The requested change operation MAY require a not-null instance of this.",
                Severity = ErrorSeverity.Warning,
                ErrorKind = ServiceErrorKinds.RequestValidation
            };

            errors.Add(error);
        }

        return errors;
    }
}

internal class SubscriptionValidator : IValidator<CustomerSubscription>
{
    public IEnumerable<ServiceError> Validate(CustomerSubscription? instance)
    {
        List<ServiceError> errors = new();

        if (instance == null)
        {
            ServiceError error = new()
            {
                Message = "CustomerSubscription instance is null.",
                Severity = ErrorSeverity.Warning,
                ErrorKind = ServiceErrorKinds.RequestValidation
            };

            errors.Add(error);
            // instance null.  Nothing else makes sense.  Return early.
            return errors;
        }

        

        return errors;
    }

    public IEnumerable<ServiceError> Validate(CustomerSubscription? instance, Func<CustomerSubscription?, IEnumerable<ServiceError>> contextualDelegate)
    {
        return contextualDelegate(instance);
    }
}

internal class SubscriptionActionValidator: IValidator<SubscriptionActionDetail>
{
    /// <summary>
    /// Validates the SubscriptionActionDetail instance to ensure that it
    /// is well-formed and the data is "internally" correct for the object.
    /// </summary>
    /// <param name="instance"></param>
    /// <returns></returns>
    public IEnumerable<ServiceError> Validate(SubscriptionActionDetail? instance)
    {
        List<ServiceError> errors = new();

        if (instance == null)
        {
            ServiceError error = new()
            {
                Message = "SubscriptionActionDetail instance is null.",
                Severity = ErrorSeverity.Error,
                ErrorKind = ServiceErrorKinds.RequestValidation
            };

            errors.Add(error);
            // instance null.  Nothing else makes sense.  Return early.
            return errors;
        }

        //CustomerProfileId is required.
        if(string.IsNullOrWhiteSpace(instance.CustomerProfileId))
        {
            ServiceError error = new()
            {
                Message = "SubscriptionActionDetail.CustomerProfileId is null or empty.",
                Severity = ErrorSeverity.Error,
                ErrorKind = ServiceErrorKinds.RequestValidation
            };

            errors.Add(error);
        }

        // ActionName is required.
        if (string.IsNullOrWhiteSpace(instance.ActionName))
        {
            ServiceError error = new()
            {
                Message = "SubscriptionActionDetail.ActionKind is null or empty.",
                Severity = ErrorSeverity.Error,
                ErrorKind = ServiceErrorKinds.RequestValidation
            };

            errors.Add(error);
        }

        // ActionName must be a well-known SubscirptionChangeKind.
        if(SubscriptionChangeKinds.AllowedValues.Contains(instance.ActionName) == false)
        {
            ServiceError error = new()
            {
                Message = $"'{instance.ActionName}' is not a valid SubscriptionChangeKind.",
                Severity = ErrorSeverity.Error,
                ErrorKind = ServiceErrorKinds.RequestValidation
            };

            errors.Add(error);
        }

        // RequestSource is required.
        if(string.IsNullOrWhiteSpace(instance.RequestSource))
        {
            ServiceError error = new()
            {
                Message = "SubscriptionActionDetail.RequestSource is null or empty.",
                Severity = ErrorSeverity.Error,
                ErrorKind = ServiceErrorKinds.RequestValidation
            };

            errors.Add(error);
        }

        // RequestSource must be a well-known value.
        if(ChangeRequestSource.AllowedValues.Contains(instance.RequestSource) == false)
        {
            ServiceError error = new()
            {
                Message = $"'{instance.RequestSource}' is not a valid RequestSource.",
                Severity = ErrorSeverity.Error,
                ErrorKind = ServiceErrorKinds.RequestValidation
            };

            errors.Add(error);
        }

        // VendorSuppliedCustomerId is required.
        if(string.IsNullOrWhiteSpace(instance.VendorSuppliedCustomerId))
        {
            ServiceError error = new()
            {
                Message = "SubscriptionActionDetail.VendorSuppliedCustomerId is null or empty.",
                Severity = ErrorSeverity.Error,
                ErrorKind = ServiceErrorKinds.RequestValidation
            };

            errors.Add(error);
        }

        // VendorName is required.
        if(string.IsNullOrWhiteSpace(instance.VendorName))
        {
            ServiceError error = new()
            {
                Message = "SubscriptionActionDetail.VendorName is null or empty.",
                Severity = ErrorSeverity.Error,
                ErrorKind = ServiceErrorKinds.RequestValidation
            };

            errors.Add(error);
        }

        // VendorName must be well-known.
        if(ExternalServiceVendors.AllowedValues.Contains(instance.VendorName) == false)
        {
            ServiceError error = new()
            {
                Message = $"'{instance.VendorName}' is not a valid VendorName.",
                Severity = ErrorSeverity.Error,
                ErrorKind = ServiceErrorKinds.RequestValidation
            };

            errors.Add(error);
        }

        // SubscriptionSku is required.
        if(string.IsNullOrWhiteSpace(instance.SubscriptionSku))
        {
            ServiceError error = new()
            {
                Message = "SubscriptionActionDetail.SubscriptionSku is null or empty.",
                Severity = ErrorSeverity.Error,
                ErrorKind = ServiceErrorKinds.RequestValidation
            };

            errors.Add(error);
        }

        // SubscriptionSku must be a well-known value.
        if(SubscriptionIdentifiers.AllowedSkus.Contains(instance.SubscriptionSku) == false)
        {
            ServiceError error = new()
            {
                Message = $"'{instance.SubscriptionSku}' is not a valid SubscriptionSku.",
                Severity = ErrorSeverity.Error,
                ErrorKind = ServiceErrorKinds.RequestValidation
            };

            errors.Add(error);
        }

        return errors;
    }

    /// <summary>
    /// Proxies back to custom rules defined in the contextualDelegate to ensure that
    /// the SubscriptionActionDetail is appropriate for the context in which it is operating.
    /// </summary>
    /// <param name="instance"></param>
    /// <param name="contextualDelegate"></param>
    /// <returns></returns>
    public IEnumerable<ServiceError> Validate(SubscriptionActionDetail? instance, Func<SubscriptionActionDetail?, IEnumerable<ServiceError>> contextualDelegate)
    {
        return contextualDelegate(instance);
    }
}