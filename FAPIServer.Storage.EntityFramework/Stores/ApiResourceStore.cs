using FAPIServer.Storage.Models;
using FAPIServer.Storage.Stores;
using Microsoft.EntityFrameworkCore;

namespace FAPIServer.Storage.EntityFramework.Stores;

public class ApiResourceStore : IApiResourceStore
{
    private readonly ConfigurationDbContext _dbContext;

    public ApiResourceStore(ConfigurationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ApiResource?> FindByClientIdAsync(string clientId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.ApiResources.AsNoTracking()
            .Where(p => p.Enabled && p.Client.ClientId == clientId)
            .Select(p => new ApiResource
            {
                Uri = p.Uri,
                Enabled = p.Enabled,
                ClientId = p.Client.ClientId,
                HandledAuthorizationDetailTypes = p.AuthorizationDetailSchemas.Select(p => p.Type)
            })
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<ApiResource>> FindByUrisAsync(IEnumerable<Uri> uris, CancellationToken cancellationToken = default)
    {
        return await _dbContext.ApiResources.AsNoTracking()
            .Where(p => p.Enabled && uris.Contains(p.Uri))
            .Select(p => new ApiResource
            {
                Uri = p.Uri,
                Enabled = p.Enabled,
                ClientId = p.Client.ClientId,
                HandledAuthorizationDetailTypes = p.AuthorizationDetailSchemas.Select(p => p.Type)
            })
            .ToListAsync(cancellationToken);
    }
}
