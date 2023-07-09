using FAPIServer.RequestHandling.Requests;
using FAPIServer.Storage.Models;
using FAPIServer.Storage.ValueObjects;

namespace FAPIServer.Validation.Models;

public class ValidatedCibaRequest : ValidatedRequest<CibaRequest>
{
    public ValidatedCibaRequest(CibaRequest rawRequest, Client client,
        IEnumerable<AuthorizationDetail>? authorizationDetails,
        IEnumerable<string>? claims,
        string subject,
        Grant? requestedGrant = null)
        : base(rawRequest, client)
    {
        if (string.IsNullOrEmpty(subject))
            throw new ArgumentException($"'{nameof(subject)}' cannot be null or empty.", nameof(subject));

        AuthorizationDetails = authorizationDetails ?? Array.Empty<AuthorizationDetail>();
        Claims = claims ?? Array.Empty<string>();
        Subject = subject;
        RequestedGrant = requestedGrant;
    }

    public IEnumerable<AuthorizationDetail> AuthorizationDetails { get; init; }
    public IEnumerable<string> Claims { get; init; }
    public string Subject { get; init; }
    public Grant? RequestedGrant { get; init; }
}
