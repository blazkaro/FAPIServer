using FAPIServer.RequestHandling.Requests;
using FAPIServer.Storage.Models;

namespace FAPIServer.Validation.Models;

public class ValidatedAuthorizationRequest : ValidatedRequest<AuthorizationRequest>
{
    public ValidatedAuthorizationRequest(AuthorizationRequest rawRequest, Client client, ParObject parObject)
        : base(rawRequest, client)
    {
        ParObject = parObject ?? throw new ArgumentNullException(nameof(parObject));
    }

    public ValidatedAuthorizationRequest(AuthorizationRequest rawRequest, Client client, ParObject parObject, IEnumerable<AuthorizationDetailSchema>? involvedSchemas)
    : this(rawRequest, client, parObject)
    {
        AuthorizationDetailSchemas = involvedSchemas ?? Array.Empty<AuthorizationDetailSchema>();
    }

    public ParObject ParObject { get; init; }
    public IEnumerable<AuthorizationDetailSchema> AuthorizationDetailSchemas { get; init; } = Array.Empty<AuthorizationDetailSchema>();
}
