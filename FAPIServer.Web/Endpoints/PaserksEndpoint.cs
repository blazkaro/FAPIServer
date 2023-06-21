using FAPIServer.Storage.Stores;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Paseto;
using Paseto.Cryptography.Key;
using Paseto.Protocol;
using System.Text.Json.Serialization;

namespace FAPIServer.Web.Endpoints;

[Route("fapi/paserks")]
[ResponseCache(Duration = 1800, Location = ResponseCacheLocation.Any, NoStore = false)]
public class PaserksEndpoint : Endpoint
{
    private readonly ISecretCredentialsStore _secretCredentialsStore;
    private readonly IMemoryCache _memoryCache;

    public PaserksEndpoint(ISecretCredentialsStore secretCredentialsStore,
        IMemoryCache memoryCache)
    {
        _secretCredentialsStore = secretCredentialsStore;
        _memoryCache = memoryCache;
    }

    public override async Task<IActionResult> HandleAsync(CancellationToken cancellationToken)
    {
        if (!_memoryCache.TryGetValue("paserks", out OkObjectResult? cachedResult) || cachedResult is null)
        {
            var publicKey = (await _secretCredentialsStore.GetSigningCredentials(false, cancellationToken)).PublicKey.Value;
            var paserk = Paserk.Encode(new PasetoAsymmetricPublicKey(publicKey, new Version4()), PaserkType.Public);

            var result = new OkObjectResult(new ResultDto { Keys = new string[] { paserk } });
            lock (_memoryCache)
            {
                _memoryCache.Set("paserks", result, DateTime.UtcNow.AddSeconds(1800));
            }

            return result;
        }

        return cachedResult;
    }

    private sealed class ResultDto
    {
        [JsonPropertyName("keys")]
        public string[] Keys { get; set; }
    }
}
