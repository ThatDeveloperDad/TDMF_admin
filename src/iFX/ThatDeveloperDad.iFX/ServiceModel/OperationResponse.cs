using System;
using System.Collections.Generic;
using System.Linq;

namespace ThatDeveloperDad.iFX.ServiceModel;

public abstract class OperationResponse
{
    protected  List<ServiceError> _errorsCollection = new();
    public OperationRequest? Request { get; protected set; }

    public bool HasWarnings => _errorsCollection.Any(e => e.Severity == ErrorSeverity.Warning);

    public bool HasErrors => _errorsCollection.Any(e => e.Severity == ErrorSeverity.Error);

    // Yes, this IS just a repetion of HasErrors, I've added this for improved Semantic Clarity.
    public bool ShouldHaltProcess => _errorsCollection.Any(e => e.Severity == ErrorSeverity.Error);

    public void AddError(ServiceError error)
    {
        _errorsCollection.Add(error);
    }

    public void AddErrors(OperationResponse donor, string errorSite = "")
    {
        // Update the Site on any incoming errors only if errorSite has a value, and their Site property is empty.
        var errors = UpdateErrorSites(donor._errorsCollection, errorSite);
        _errorsCollection.AddRange(errors);
    }
//TODO:  Port to system-framework repo
    public void AddErrors(IEnumerable<ServiceError> errors, string errorSite = "")
    {
        // Update the Site on any incoming errors only if errorSite their Site property is empty.
        errors = UpdateErrorSites(errors, errorSite);
        _errorsCollection.AddRange(errors);
    }

    public string?[] ErrorReport => _errorsCollection.Select(e => e.ToString()).ToArray();

//TODO:  Hey Dude!  Pull this change over to the main Framework repo later.
    public bool HasErrorKind(string errorKind)
    {
        return _errorsCollection.Any(e => e.ErrorKind == errorKind);
    }

    public void ClearErrorKind(string errorKind)
    {
        _errorsCollection.RemoveAll(e => e.ErrorKind == errorKind);
    }

    private IEnumerable<ServiceError> UpdateErrorSites(IEnumerable<ServiceError> errors, string errorSite)
    {
        if(string.IsNullOrWhiteSpace(errorSite))
        {
            return errors;
        }

        foreach(ServiceError error in errors.Where(e => string.IsNullOrWhiteSpace(e.Site)))
        {
            error.Site = errorSite;
        }

        return errors;
    }

}

public abstract class OperationResponse<T>:OperationResponse
{
        public OperationResponse(OperationRequest request, T? payload)
    {
        Request = request;
        Payload = payload;
    }
        
    public bool Successful => HasErrors==false && Payload!=null;

    public T? Payload { get; set; }

}
