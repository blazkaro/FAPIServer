using FAPIServer.Storage.Models;

namespace FAPIServer.Validation.Results;

public class GrantManagementValidationResult : ValidationResultBase
{
    public GrantManagementValidationResult()
    {
        
    }

    public GrantManagementValidationResult(Error? error) : base(error)
    {
    }

    public GrantManagementValidationResult(Error? error, string? failureMessage) : base(error, failureMessage)
    {
    }

    public GrantManagementValidationResult(Grant? grant)
    {
        IsValid = true;
        Grant = grant;
    }

    public Grant? Grant { get; init; }
}
