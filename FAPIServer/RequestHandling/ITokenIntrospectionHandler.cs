using FAPIServer.RequestHandling.Contexts;
using FAPIServer.RequestHandling.Results;

namespace FAPIServer.RequestHandling;

public interface ITokenIntrospectionHandler
{
    Task<TokenIntrospectionHandlerResult> HandleAsync(TokenIntrospectionContext context, CancellationToken cancellationToken = default);
}
