using FAPIServer.RequestHandling.Contexts;

namespace FAPIServer.RequestHandling;

public interface IGrantRevocationHandler
{
    Task<bool> HandleAsync(GrantManagementContext context, CancellationToken cancellationToken = default);
}
