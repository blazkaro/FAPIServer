using FAPIServer.RequestHandling.Requests;
using FAPIServer.Storage.Models;
using FAPIServer.Storage.ValueObjects;

namespace FAPIServer.Validation.Models;

public class ValidatedPushedAuthorizationRequest : ValidatedRequest<PushedAuthorizationRequest>
{
    public ValidatedPushedAuthorizationRequest(PushedAuthorizationRequest rawRequest, Client client,
        IEnumerable<AuthorizationDetail>? authorizationDetails,
        IEnumerable<AuthorizationDetailSchema>? authorizationDetailSchemas,
        IEnumerable<string>? claims,
        Grant? requestedGrant = null)
        : base(rawRequest, client)
    {
        AuthorizationDetails = authorizationDetails ?? Array.Empty<AuthorizationDetail>();
        AuthorizationDetailSchemas = authorizationDetailSchemas ?? Array.Empty<AuthorizationDetailSchema>();
        Claims = claims ?? Array.Empty<string>();
        RequestedGrant = requestedGrant;
    }

    public IEnumerable<AuthorizationDetail> AuthorizationDetails { get; init; }
    public IEnumerable<AuthorizationDetailSchema> AuthorizationDetailSchemas { get; init; }
    public IEnumerable<string> Claims { get; init; }
    public Grant? RequestedGrant { get; init; }
}
