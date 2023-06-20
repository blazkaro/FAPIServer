using Base64Url;
using FAPIServer.Helpers;
using FAPIServer.RequestHandling.Requests;
using FAPIServer.Services;
using FAPIServer.Storage.Models;
using FAPIServer.Storage.Stores;
using Microsoft.AspNetCore.Http;
using System.Security.Cryptography;

namespace FAPIServer.Web.Services;

/// <summary>
/// Authorization request persistence service implementation that relies on <see cref="IParObjectStore"/> and session
/// </summary>
public class AuthorizationRequestPersistenceService : IAuthorizationRequestPersistenceService
{
    private readonly IParObjectStore _parObjectStore;
    private readonly IHttpContextAccessor _contextAccessor;

    public AuthorizationRequestPersistenceService(IParObjectStore parObjectStore,
        IHttpContextAccessor contextAccessor)
    {
        _parObjectStore = parObjectStore;
        _contextAccessor = contextAccessor;
    }

    private const string SESSION_COOKIE_NAME = "asid";
    private readonly static CookieOptions SESSION_COOKIE_OPTIONS = new CookieOptions
    {
        HttpOnly = true,
        IsEssential = true,
        Path = "/",
        Secure = true,
        SameSite = SameSiteMode.Strict
    };

    public async Task PersistAsync(ParObject parObject, CancellationToken cancellationToken = default)
    {
        var httpContext = _contextAccessor.HttpContext ?? throw new InvalidOperationException("The HttpContext is null");

        // As it relies on IParObjectStore, we know that the ParObject is persisted after successful Pushed Authorization Request
        // So only thing we have to do here is to mark ParObject as activated and establish session
        var sid = Base64UrlEncoder.Encode(RandomNumberGenerator.GetBytes(64));
        await UpdateAsync(parObject, update =>
        {
            update.HasBeenActivated = true;
            update.Sid = sid;
        }, cancellationToken);

        httpContext.Response.Cookies.Append(SESSION_COOKIE_NAME, sid, SESSION_COOKIE_OPTIONS);
    }

    public async Task<ParObject?> ReadAsync(AuthorizationRequest request, CancellationToken cancellationToken = default)
    {
        var httpContext = _contextAccessor.HttpContext ?? throw new InvalidOperationException("The HttpContext is null");

        var requestUri = InteractionHelper.RemoveRequestUriPrefix(request.RequestUri);
        var parObject = await _parObjectStore.FindByUriAndClientIdAsync(requestUri, request.ClientId, cancellationToken);
        if (parObject is null)
            return null;

        if (parObject.HasBeenActivated && parObject.Sid != httpContext.Request.Cookies[SESSION_COOKIE_NAME])
            return null;

        return parObject;
    }

    public async Task RemoveAsync(ParObject parObject, CancellationToken cancellationToken = default)
    {
        var httpContext = _contextAccessor.HttpContext ?? throw new InvalidOperationException("The HttpContext is null");

        await _parObjectStore.RemoveAsync(parObject.Uri, cancellationToken);
        httpContext.Response.Cookies.Delete(SESSION_COOKIE_NAME, SESSION_COOKIE_OPTIONS);
    }

    public async Task UpdateAsync(ParObject parObject, Action<ParObject> update, CancellationToken cancellationToken = default)
    {
        await _parObjectStore.UpdateAsync(parObject.Uri, update, cancellationToken);
    }
}
