using FAPIServer.Storage;
using FAPIServer.Storage.Models;
using FAPIServer.Storage.Stores;
using FAPIServer.Storage.ValueObjects;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace FAPIServer.Services.Default;

public class CibaInteractionService : ICibaInteractionService
{
    private readonly IGrantManager _grantManager;
    private readonly IChangesTracker<CibaObject> _changesTracker;
    private readonly IClientStore _clientStore;
    private readonly IHttpClientFactory _httpClientFactory;

    public CibaInteractionService(IGrantManager grantManager,
        IChangesTracker<CibaObject> changesTracker,
        IClientStore clientStore,
        IHttpClientFactory httpClientFactory)
    {
        _grantManager = grantManager;
        _changesTracker = changesTracker;
        _clientStore = clientStore;
        _httpClientFactory = httpClientFactory;
    }

    public async Task DenyConsentAsync(CibaObject cibaObject, CancellationToken cancellationToken = default)
    {
        if (cibaObject is null)
            throw new ArgumentNullException(nameof(cibaObject));

        _changesTracker.BeginTracking(cibaObject);

        cibaObject.AccessDenied = true;
        cibaObject.IsCompleted = true;

        await _changesTracker.SaveChangesAsync(cancellationToken);
        await SendAuthenticationResult(cibaObject, cancellationToken);
    }

    public async Task GrantConsentAsync(CibaObject cibaObject, CancellationToken cancellationToken = default)
    {
        if (cibaObject is null)
            throw new ArgumentNullException(nameof(cibaObject));

        await GrantConsentAsync(cibaObject, cibaObject.AuthorizationDetails, cibaObject.Claims, cancellationToken);
    }

    public async Task GrantConsentAsync(CibaObject cibaObject, IEnumerable<AuthorizationDetail>? authorizationDetails, IEnumerable<string>? claims,
        CancellationToken cancellationToken = default)
    {
        if (cibaObject is null)
            throw new ArgumentNullException(nameof(cibaObject));

        authorizationDetails ??= Array.Empty<AuthorizationDetail>();
        claims ??= Array.Empty<string>();

        if (!authorizationDetails.Any() && !claims.Any())
        {
            await DenyConsentAsync(cibaObject, cancellationToken);
            return;
        }

        _changesTracker.BeginTracking(cibaObject);
        cibaObject.IsCompleted = true;
        await _changesTracker.SaveChangesAsync(cancellationToken);

        switch (cibaObject.GrantManagementAction)
        {
            case Constants.GrantManagementActions.Create or null or "":
                cibaObject.Grant = await _grantManager.CreateAsync(cibaObject.ClientId, cibaObject.Subject, authorizationDetails, claims, cancellationToken);
                break;

            case Constants.GrantManagementActions.Merge:
                await _grantManager.MergeAsync(cibaObject.Grant!, authorizationDetails, claims, cancellationToken);
                break;

            case Constants.GrantManagementActions.Replace:
                await _grantManager.ReplaceAsync(cibaObject.Grant!, authorizationDetails, claims, cancellationToken);
                break;
        }

        await SendAuthenticationResult(cibaObject, cancellationToken);
    }

    private async Task SendAuthenticationResult(CibaObject cibaObject, CancellationToken cancellationToken = default)
    {
        var client = await _clientStore.FindEnabledByClientIdAsync(cibaObject.ClientId, cancellationToken);

        // Currently only supported mode that requires callback from authorization server is Ping mode
        if (client is null || client.CibaMode != Constants.CibaModes.Ping)
            return;

        HttpContent httpContent = JsonContent.Create(new PingModeDto { AuthReqId = cibaObject.Id });
        httpContent.Headers.Add("Authorization", $"Bearer {cibaObject.ClientNotificationToken}");

        var httpClient = _httpClientFactory.CreateClient(Constants.CibaNotificationHttpClientName);
        await httpClient.PostAsync(client.BackchannelClientNotificationEndpoint, httpContent, cancellationToken);
    }

    private sealed class PingModeDto
    {
        [JsonPropertyName("auth_req_id")]
        public string AuthReqId { get; set; }
    }
}
