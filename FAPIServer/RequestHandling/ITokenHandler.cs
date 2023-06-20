using FAPIServer.RequestHandling.Contexts;
using FAPIServer.RequestHandling.Results;

namespace FAPIServer.RequestHandling;

public interface ITokenHandler
{
    Task<TokenHandlerResult> HandleAsync(TokenContext context, CancellationToken cancellationToken = default);
}
