using FAPIServer.RequestHandling.Contexts;
using FAPIServer.RequestHandling.Results;

namespace FAPIServer.RequestHandling;

public interface IGrantQueryingHandler
{
    Task<GrantQueryingHandlerResult> HandleAsync(GrantManagementContext context, CancellationToken cancellationToken = default);
}
