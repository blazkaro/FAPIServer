using Base64Url;
using FAPIServer.Extensions;
using FAPIServer.Storage.Stores;
using FAPIServer.Web.Endpoints.Results;
using FAPIServer.Web.Extensions;
using Microsoft.AspNetCore.Mvc.Filters;
using NSec.Cryptography;
using Paseto;
using Paseto.Builder;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FAPIServer.Web.Attributes;

public class SecureResponseAttribute : ActionFilterAttribute
{
    private readonly ISecretCredentialsStore _secretCredentialsStore;

    public SecureResponseAttribute(ISecretCredentialsStore secretCredentialsStore)
    {
        _secretCredentialsStore = secretCredentialsStore;
    }

    public override async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        if (context.Result is FapiWebSecureActionResult result
            && context.HttpContext.Items.TryGetValue(WebConstants.ResponseAudienceKey, out object? value) && value is string audience && !audience.IsNullOrEmpty())
        {
            var ed25519KeyPair = await _secretCredentialsStore.GetSigningCredentials(true, context.HttpContext.RequestAborted);
            var utcNow = DateTime.UtcNow;

            var token = new PasetoBuilder()
                .UseV4(Purpose.Public)
                .WithSecretKey(ed25519KeyPair.PrivateKey.Value.Concat(ed25519KeyPair.PublicKey.Value).ToArray())
                .Issuer(context.HttpContext.Request.GetRequestedEndpointUri().ToString())
                .Audience(audience)
                .Subject(audience)
                .NotBefore(utcNow)
                .Expiration(utcNow.AddSeconds(15))
                .AddClaim("hash", Base64UrlEncoder.Encode(CreateResponseHash(result)))
                .Encode();

            context.HttpContext.Response.Headers.Add(WebConstants.BodySecurityTokenHeader, token);
        }

        await next();
    }

    private static byte[] CreateResponseHash(FapiWebSecureActionResult result)
    {
        var responseBodyBytes = JsonSerializer.SerializeToUtf8Bytes(result.GetResult(),
            new JsonSerializerOptions(JsonSerializerOptions.Default)
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            });

        var responseBodyHash = HashAlgorithm.Sha256.Hash(responseBodyBytes);

        return responseBodyHash;
    }
}
