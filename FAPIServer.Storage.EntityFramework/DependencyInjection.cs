using FAPIServer.Storage.EntityFramework.Stores;
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
        services.TryAddScoped<IRevokedAccessTokenStore, RevokedAccessTokenStore>();
        services.TryAddScoped<IRevokedClientAssertionStore, RevokedClientAssertionStore>();

        return services;
    }
}
