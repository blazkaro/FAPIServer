using FAPIServer.Authentication;
using FAPIServer.Authentication.Default;
using FAPIServer.RequestHandling;
using FAPIServer.RequestHandling.Default;
using FAPIServer.ResponseHandling;
using FAPIServer.ResponseHandling.Default;
using FAPIServer.Services;
using FAPIServer.Services.Default;
using FAPIServer.Validation;
using FAPIServer.Validation.Default;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace FAPIServer.Extensions;

public static class DependencyInjection
{
    public static FapiServerBuilder AddFapiServer(this IServiceCollection services, Action<FapiOptions>? options = null)
    {
        options ??= cfg => { };
        services.Configure(options);

        services.TryAddTransient<IAccessTokenService, AccessTokenService>();
        services.TryAddTransient<IIdTokenService, IdTokenService>();
        services.TryAddTransient<IRefreshTokenService, RefreshTokenService>();

        services.TryAddScoped<IGrantManager, GrantManager>();
        services.TryAddScoped<IInteractionService, InteractionService>();
        services.TryAddScoped<ICibaInteractionService, CibaInteractionService>();

        services.TryAddScoped<IClientAuthenticator, ClientAuthenticator>();

        services.TryAddScoped<IPushedAuthorizationHandler, PushedAuthorizationHandler>();
        services.TryAddScoped<IAuthorizationHandler, AuthorizationHandler>();
        services.TryAddScoped<ITokenHandler, TokenHandler>();
        services.TryAddScoped<ITokenIntrospectionHandler, TokenIntrospectionHandler>();
        services.TryAddScoped<ITokenRevocationHandler, TokenRevocationHandler>();
        services.TryAddScoped<IGrantQueryingHandler, GrantQueryingHandler>();
        services.TryAddScoped<IGrantRevocationHandler, GrantRevocationHandler>();
        services.TryAddScoped<IUserInfoHandler, UserInfoHandler>();
        services.TryAddScoped<ICibaHandler, CibaHandler>();

        services.TryAddScoped<IGrantManagementValidator, GrantManagementValidator>();
        services.TryAddScoped<IResourceValidator, ResourceValidator>();
        services.TryAddScoped<IDPoPProofValidator, DPoPProofValidator>();
        services.TryAddScoped<IAccessTokenValidator, AccessTokenValidator>();
        services.TryAddScoped<IIdTokenValidator, IdTokenValidator>();

        services.TryAddScoped<IPushedAuthorizationRequestValidator, PushedAuthorizationRequestValidator>();
        services.TryAddScoped<IAuthorizationRequestValidator, AuthorizationRequestValidator>();
        services.TryAddScoped<ITokenRequestValidator, TokenRequestValidator>();
        services.TryAddScoped<ICibaRequestValidator, CibaRequestValidator>();

        services.TryAddScoped<IPushedAuthorizationResponseGenerator, PushedAuthorizationResponseGenerator>();
        services.TryAddScoped<IAuthorizationResponseGenerator, AuthorizationResponseGenerator>();
        services.TryAddScoped<ITokenResponseGenerator, TokenResponseGenerator>();
        services.TryAddScoped<ITokenIntrospectionResponseGenerator, TokenIntrospectionResponseGenerator>();
        services.TryAddScoped<ICibaResponseGenerator, CibaResponseGenerator>();

        services.AddHttpClient(Constants.CibaNotificationHttpClientName)
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                return new HttpClientHandler()
                {
                    AllowAutoRedirect = false,
                    UseCookies = false
                };
            });

        return new FapiServerBuilder(services);
    }
}
