using FAPIServer.Services.Models;

namespace FAPIServer.Services;

public interface IUserService
{
    Task<IDictionary<string, object>> GetClaimsAsync(string subject, IEnumerable<string> requestedClaims, CancellationToken cancellationToken = default);
    Task<UserCibaContext> GetCibaContextAsync(string subject, string? userCode, CancellationToken cancellationToken = default);
}
