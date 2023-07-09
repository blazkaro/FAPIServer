using FAPIServer.RequestHandling.Contexts;
using FAPIServer.RequestHandling.Results;

namespace FAPIServer.RequestHandling;

public interface ICibaHandler
{
    Task<CibaHandlerResult> HandleAsync(CibaContext context, CancellationToken cancellationToken = default);
}
