using FAPIServer.Storage.Models;
using FAPIServer.Storage.ValueObjects;

namespace FAPIServer.Validation.Results;

public class ResourceValidationResult : ValidationResultBase
{
    public ResourceValidationResult(Error? error) : base(error)
    {
    }

    public ResourceValidationResult(Error? error, string? failureMessage) : base(error, failureMessage)
    {
    }

    public ResourceValidationResult(IEnumerable<AuthorizationDetail>? authorizationDetails, IEnumerable<AuthorizationDetailSchema>? involvedSchemas)
    {
        IsValid = true;
        AuthorizationDetails = authorizationDetails ?? Array.Empty<AuthorizationDetail>();
        InvolvedSchemas = involvedSchemas ?? Array.Empty<AuthorizationDetailSchema>();
    }

    public ResourceValidationResult(IEnumerable<string>? claims)
    {
        IsValid = true;
        Claims = claims ?? Array.Empty<string>();
    }

    public ResourceValidationResult(IEnumerable<AuthorizationDetail>? authorizationDetails, IEnumerable<AuthorizationDetailSchema>? involvedSchemas, IEnumerable<string>? claims)
        : this(authorizationDetails, involvedSchemas)
    {
        IsValid = true;
        Claims = claims ?? Array.Empty<string>();
    }

    public IEnumerable<AuthorizationDetail> AuthorizationDetails { get; set; } = Array.Empty<AuthorizationDetail>();
    public IEnumerable<AuthorizationDetailSchema> InvolvedSchemas { get; set; } = Array.Empty<AuthorizationDetailSchema>();
    public IEnumerable<string> Claims { get; set; } = Array.Empty<string>();
}
