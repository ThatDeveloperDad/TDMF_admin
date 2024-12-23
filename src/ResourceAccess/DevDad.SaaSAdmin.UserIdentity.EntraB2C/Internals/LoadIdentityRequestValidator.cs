using System;
using DevDad.SaaSAdmin.iFX;
using DevDad.SaaSAdmin.UserIdentity.Abstractions;
using ThatDeveloperDad.iFX.ObjectUtilities;
using ThatDeveloperDad.iFX.ServiceModel;

namespace DevDad.SaaSAdmin.UserIdentity.EntraB2C.Internals;

public class LoadIdentityRequestValidator : IValidator<LoadIdentityRequest>
{
    public IEnumerable<ServiceError> Validate(LoadIdentityRequest? instance)
    {
        List<ServiceError> errors = new();

        if(instance == null)
        {
            errors.Add(new ServiceError
            {
                Severity = ErrorSeverity.Error,
                Message = "The request instance is required.",
                ErrorKind = ServiceErrorKinds.RequestValidation,
            });
            return errors;
        }

        if(string.IsNullOrWhiteSpace(instance.UserId))
        {
            errors.Add(new ServiceError
            {
                Severity = ErrorSeverity.Error,
                Message = "The UserId is required to load a user identity.",
                ErrorKind = ServiceErrorKinds.RequestPayloadValidation,
            });
        }

        return errors;
    }

}
