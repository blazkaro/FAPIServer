using FAPIServer.RequestHandling.Contexts;
using FAPIServer.RequestHandling.Results;

namespace FAPIServer.RequestHandling;

public interface ITokenRevocationHandler
{
    Task<TokenRevocationHandlerResult> HandleAsync(TokenRevocationContext context, CancellationToken cancellationToken = default);
}
