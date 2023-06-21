using Base64Url;
using FAPIServer.Extensions;
using FAPIServer.Helpers;
using FAPIServer.Models;
using FAPIServer.Serializers;
using FAPIServer.Validation.Contexts;
using FAPIServer.Validation.Results;
using NSec.Cryptography;
using Paseto;
using Paseto.Builder;
using Paseto.Cryptography.Key;
using System.Text;

namespace FAPIServer.Validation.Default;

public class DPoPProofValidator : IDPoPProofValidator
{
    public TokenValidationResult<DPoPPayload> Validate(string dpop, DPoPValidationParameters parameters)
    {
        if (string.IsNullOrEmpty(dpop))
            throw new ArgumentException($"'{nameof(dpop)}' cannot be null or empty.", nameof(dpop));

        if (parameters is null)
            throw new ArgumentNullException(nameof(parameters));

        if (!PasetoHelper.IsV4Public(dpop))
            return new("The DPoP is not valid V4 public PASETO");

        var paserk = new PasetoBuilder()
            .UseV4(Purpose.Public)
            .DecodeFooter(dpop);

        if (!PaserkHelper.IsK4Public(dpop))
            return new("The footer is not valid V4 public PASERK");

        if (parameters.ValidPkh is not null && parameters.ValidPkh.Any() && !HashAlgorithm.Sha256.Verify(Encoding.UTF8.GetBytes(paserk), parameters.ValidPkh))
            return new("The footer does not match required PASERK");

        PasetoKey pasetoKey = Paserk.Decode(paserk);
        var validationResult = new PasetoBuilder()
            .WithJsonSerializer(new PasetoPayloadSerializer())
            .UseV4(Purpose.Public)
            .WithKey(pasetoKey)
            .Decode(dpop, new PasetoTokenValidationParameters
            {
                ValidateLifetime = true,
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateSubject = false
            });

        if (!validationResult.IsValid)
            return new(validationResult.Exception.Message);

        var payload = DPoPPayload.FromJson(validationResult.Paseto.RawPayload)!;
        if (payload.NotBefore == default)
            return TokenValidationResult<DPoPPayload>.MissingClaim(PasetoRegisteredClaimNames.NotBefore);

        if (payload.Jti == Guid.Empty)
            return TokenValidationResult<DPoPPayload>.MissingClaim(PasetoRegisteredClaimNames.TokenIdentifier);

        if (payload.Htm.IsNullOrEmpty()) return TokenValidationResult<DPoPPayload>.MissingClaim("htm");
        if (payload.Htu.IsNullOrEmpty()) return TokenValidationResult<DPoPPayload>.MissingClaim("htu");

        if (payload.Htm.Equals(parameters.ValidHtm, StringComparison.OrdinalIgnoreCase))
            return TokenValidationResult<DPoPPayload>.InvalidClaim("htm");

        if (!payload.Htu.Equals(parameters.ValidHtu.ToString(), StringComparison.OrdinalIgnoreCase))
            return TokenValidationResult<DPoPPayload>.InvalidClaim("htu");

        if (parameters.ValidAth is not null)
        {
            if (payload.Ath.IsNullOrEmpty()) return TokenValidationResult<DPoPPayload>.MissingClaim("ath");

            if (payload.Ath != Base64UrlEncoder.Encode(parameters.ValidAth))
                return TokenValidationResult<DPoPPayload>.InvalidClaim("ath");
        }

        return new(payload);
    }
}
