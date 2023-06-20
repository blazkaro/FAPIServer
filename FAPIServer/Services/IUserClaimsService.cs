namespace FAPIServer.Services;

public interface IUserClaimsService
{
    Task<IDictionary<string, object>> GetClaims(string subject, IEnumerable<string> requestedClaims, CancellationToken cancellationToken = default);
}
