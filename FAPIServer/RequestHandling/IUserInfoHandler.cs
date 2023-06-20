using FAPIServer.Models;

namespace FAPIServer.RequestHandling;

public interface IUserInfoHandler
{
    Task<IDictionary<string, object>> HandleAsync(AccessTokenPayload atPayload, CancellationToken cancellationToken = default);
}
