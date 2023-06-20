using FAPIServer.RequestHandling.Contexts;
using FAPIServer.RequestHandling.Results;

namespace FAPIServer.RequestHandling;

public interface IPushedAuthorizationHandler
{
    Task<PushedAuthorizationHandlerResult> HandleAsync(PushedAuthorizationContext context, CancellationToken cancellationToken = default);
}
