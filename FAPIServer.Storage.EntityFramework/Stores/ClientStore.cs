using FAPIServer.Storage.Models;
using FAPIServer.Storage.Stores;
using Microsoft.EntityFrameworkCore;

namespace FAPIServer.Storage.EntityFramework.Stores;

public class ClientStore : IClientStore
{
    private readonly ConfigurationDbContext _dbContext;

    public ClientStore(ConfigurationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Client?> FindEnabledByClientIdAsync(string clientId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Clients.AsNoTracking()
            .Where(p => p.ClientId == clientId)
            .Select(p => new Client
            {
                ClientId = p.ClientId,
                Enabled = p.Enabled,
                Ed25519PublicKey = p.Ed25519PublicKey,
                ConsentRequired = p.ConsentRequired,
                AuthorizationCodeBindingToDpopKeyRequired = p.AuthorizationCodeBindingToDpopKeyRequired,
                CibaMode = p.CibaMode,
                BackchannelClientNotificationEndpoint = p.BackchannelClientNotificationEndpoint,
                RequestUriLifetime = p.RequestUriLifetime,
                AuthorizationCodeLifetime = p.AuthorizationCodeLifetime,
                IdTokenLifetime = p.IdTokenLifetime,
                AccessTokenLifetime = p.AccessTokenLifetime,
                RefreshTokenLifetime = p.RefreshTokenLifetime,
                CibaRequestLifetime = p.CibaRequestLifetime,
                SlidingRefreshToken = p.SlidingRefreshToken,
                AllowedGrantTypes = p.GrantTypes.Select(p => p.GrantType),
                RedirectUris = p.RedirectUris.Select(p => p.Uri),
                AllowedClaims = p.Claims.Select(p => p.Type),
                AllowedSchemas = p.AuthorizationDetailSchemaActions.GroupBy(p => p.AuthorizationDetailSchema.Type)
                .Select(p => new ClientAllowedSchema
                {
                    SchemaType = p.Key,
                    AllowedActions = p.Select(p => p.Name)
                }),
                DisplayName = p.DisplayName,
                LogoUri = p.LogoUri
            })
            .SingleOrDefaultAsync(cancellationToken);
    }
}
