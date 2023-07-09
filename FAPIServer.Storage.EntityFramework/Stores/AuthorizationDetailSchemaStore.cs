using FAPIServer.Storage.Models;
using FAPIServer.Storage.Stores;
using Microsoft.EntityFrameworkCore;

namespace FAPIServer.Storage.EntityFramework.Stores;

public class AuthorizationDetailSchemaStore : IAuthorizationDetailSchemaStore
{
    private readonly ConfigurationDbContext _dbContext;

    public AuthorizationDetailSchemaStore(ConfigurationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<AuthorizationDetailSchema>> FindEnabledByTypesAsync(IEnumerable<string> types, CancellationToken cancellationToken = default)
    {
        return await _dbContext.AuthorizationDetailSchemas.AsNoTracking()
            .Where(p => p.Enabled && types.Contains(p.Type))
            .Select(p => new AuthorizationDetailSchema
            {
                Type = p.Type,
                Enabled = p.Enabled,
                SupportedActions = p.AuthorizationDetailSchemaActions.Select(x => new AuthorizationDetailSchemaAction
                {
                    Name = x.Name,
                    Enabled = x.Enabled,
                    ActionSchema = x.ActionSchema,
                    UseDefaultSchema = x.UseDefaultSchema,
                    IsEnriched = x.IsEnriched,
                    DisplayName = x.DisplayName,
                    Description = x.Description
                }),
                SupportedLocations = p.ApiResources.Select(p => p.Uri),
                DefaultActionSchema = p.DefaultActionSchema,
                ExtensionsSchema = p.ExtensionsSchema,
                IsReusable = p.IsReusable,
                ShowInDiscoveryDocument = p.ShowInDiscoveryDocument,
                DisplayName = p.DisplayName,
                Description = p.Description
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<string>> FindDiscoverableAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.AuthorizationDetailSchemas.AsNoTracking()
            .Where(p => p.Enabled && p.ShowInDiscoveryDocument)
            .Select(p => p.Type)
            .ToListAsync(cancellationToken);
    }
}
