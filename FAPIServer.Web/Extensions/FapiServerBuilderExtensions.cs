using FAPIServer.Services;
using FAPIServer.Web.Authentication.PasetoDpop;
using FAPIServer.Web.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace FAPIServer.Web.Extensions;

public static class FapiServerBuilderExtensions
{
    public static FapiServerBuilder AddWebServices(this FapiServerBuilder builder, AuthenticationBuilder? authBuilder = null,
        Action<PasetoDpopAuthOptions>? pasetoDpopPotions = null)
    {
        builder.Services.AddHttpContextAccessor();
        (authBuilder ?? builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
            )
            .AddScheme<PasetoDpopAuthOptions, PasetoDpopAuthHandler>(PasetoDpopAuthDefaults.AuthenticationScheme, pasetoDpopPotions);

        builder.Services.AddMemoryCache();
        builder.Services.AddControllers();

        return builder;
    }

    public static FapiServerBuilder ConfigureWebOptions(this FapiServerBuilder builder, Action<FapiWebOptions>? options = null)
    {
        options ??= cfg => { };
        builder.Services.Configure(options);

        return builder;
    }

    public static FapiServerBuilder UseDefaultAuthorizationRequestPersistenceService(this FapiServerBuilder builder)
    {
        builder.Services.TryAddScoped<IAuthorizationRequestPersistenceService, AuthorizationRequestPersistenceService>();
        return builder;
    }
}
