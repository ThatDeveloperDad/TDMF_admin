using System;
using DevDad.SaaSAdmin.iFX;
using DevDad.SaaSAdmin.UserIdentity.Abstractions;
using ThatDeveloperDad.iFX.ObjectUtilities;
using ThatDeveloperDad.iFX.ServiceModel;

namespace DevDad.SaaSAdmin.UserIdentity.EntraB2C.Internals;

internal class ReconcileMembershipRequestValidator : IValidator<ReconcileMembershipsRequest>
{


    public IEnumerable<ServiceError> Validate(
        ReconcileMembershipsRequest instance)
    {
        var errors = new List<ServiceError>();

        if (instance == null)
        {
            errors.Add(new ServiceError
            {
                Severity = ErrorSeverity.Error,
                Message = "The request instance is required.",
                ErrorKind = ServiceErrorKinds.RequestValidation,
            });
            return errors;
        }

        var payload = instance.Payload;

        if(payload == null)
        {
            errors.Add(new ServiceError
            {
                Severity = ErrorSeverity.Error,
                Message = "The request payload is required.",
                ErrorKind = ServiceErrorKinds.RequestValidation,
            });
            return errors;
        }

        if (string.IsNullOrWhiteSpace(payload.UserId))
        {
            errors.Add(new ServiceError
            {
                Severity = ErrorSeverity.Error,
                Message = "The UserId is required to reconcile memberships.",
                ErrorKind = ServiceErrorKinds.RequestPayloadValidation,
            });
        }

        if (payload.ExpectedGroups.Any() == false)
        {
            errors.Add(new ServiceError
            {
                Severity = ErrorSeverity.Error,
                Message = "The ExpectedGroups collection must include at least one group.",
                ErrorKind = ServiceErrorKinds.RequestPayloadValidation,
            });
        }

        return errors;
    }

}
