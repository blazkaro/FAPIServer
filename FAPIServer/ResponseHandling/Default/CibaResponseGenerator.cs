using Base64Url;
using FAPIServer.Extensions;
using FAPIServer.ResponseHandling.Models;
using FAPIServer.Storage.Models;
using FAPIServer.Storage.Stores;
using FAPIServer.Storage.ValueObjects;
using FAPIServer.Validation.Models;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;

namespace FAPIServer.ResponseHandling.Default;

public class CibaResponseGenerator : ICibaResponseGenerator
{
    private readonly ICibaObjectStore _cibaObjectStore;
    private readonly FapiOptions _options;

    public CibaResponseGenerator(ICibaObjectStore cibaObjectStore, IOptionsMonitor<FapiOptions> options)
    {
        _cibaObjectStore = cibaObjectStore;
        _options = options.CurrentValue;
    }

    public async Task<CibaResponse> GenerateAsync(ValidatedCibaRequest validatedRequest, CancellationToken cancellationToken = default)
    {
        if (validatedRequest is null)
            throw new ArgumentNullException(nameof(validatedRequest));

        var cibaObject = new CibaObject
        {
            Id = Base64UrlEncoder.Encode(RandomNumberGenerator.GetBytes(32)),
            ClientId = validatedRequest.Client.ClientId,
            AuthorizationDetails = validatedRequest.AuthorizationDetails,
            Claims = validatedRequest.Claims,
            ClientNotificationToken = validatedRequest.RawRequest.ClientNotificationToken,
            Subject = validatedRequest.Subject,
            BindingMessage = validatedRequest.RawRequest.BindingMessage,
            Grant = validatedRequest.RequestedGrant,
            GrantManagementAction = validatedRequest.RawRequest.GrantManagementAction,
            DPoPPkh = !validatedRequest.RawRequest.DPoPPkh.IsNullOrEmpty() ? new Base64UrlEncodedString(validatedRequest.RawRequest.DPoPPkh) : null,
            ExpiresAt = DateTime.UtcNow.AddSeconds(validatedRequest.Client.CibaRequestLifetime.Seconds)
        };

        await _cibaObjectStore.StoreAsync(cibaObject, cancellationToken);

        var response = new CibaResponse
        {
            AuthReqId = cibaObject.Id,
            ExpiresIn = validatedRequest.Client.CibaRequestLifetime.Seconds
        };

        if (validatedRequest.Client.CibaMode == Constants.CibaModes.Poll || validatedRequest.Client.CibaMode == Constants.CibaModes.Ping)
            response.Interval = _options.CibaInterval.Seconds;

        return response;
    }
}
