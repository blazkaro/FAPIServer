using FAPIServer.Storage.EntityFramework.Services;
using FAPIServer.Storage.EntityFramework.Stores;
using FAPIServer.Storage.Models;
using FAPIServer.Storage.Stores;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace FAPIServer.Storage.EntityFramework;

public static class DependencyInjection
{
    public static IServiceCollection AddFapiStorageEntityFramework(this IServiceCollection services)
    {
        services.TryAddScoped<IApiResourceStore, ApiResourceStore>();
        services.TryAddScoped<IAuthorizationCodeStore, AuthorizationCodeStore>();
        services.TryAddScoped<IAuthorizationDetailSchemaStore, AuthorizationDetailSchemaStore>();
        services.TryAddScoped<IClaimsStore, ClaimsStore>();
        services.TryAddScoped<IClientStore, ClientStore>();
        services.TryAddScoped<IGrantStore, GrantStore>();
        services.TryAddScoped<IParObjectStore, ParObjectStore>();
        services.TryAddScoped<IRefreshTokenStore, RefreshTokenStore>();
        services.TryAddScoped<ICibaObjectStore, CibaObjectStore>();
        services.TryAddScoped<IRevokedAccessTokenStore, RevokedAccessTokenStore>();
        services.TryAddScoped<IRevokedClientAssertionStore, RevokedClientAssertionStore>();

        services.TryAddTransient<IChangesTracker<Grant>, ChangesTracker<Grant, GrantsDbContext>>();
        services.TryAddTransient<IChangesTracker<ParObject>, ChangesTracker<ParObject, GrantsDbContext>>();
        services.TryAddTransient<IChangesTracker<CibaObject>, ChangesTracker<CibaObject, GrantsDbContext>>();
        services.TryAddTransient<IChangesTracker<RefreshToken>, ChangesTracker<RefreshToken, GrantsDbContext>>();

        return services;
    }
}
