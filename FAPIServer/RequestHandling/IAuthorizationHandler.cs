using FAPIServer.RequestHandling.Contexts;
using FAPIServer.RequestHandling.Results;

namespace FAPIServer.RequestHandling;

public interface IAuthorizationHandler
{
    Task<AuthorizationHandlerResult> HandleAsync(AuthorizationContext context, CancellationToken cancellationToken = default);
}
