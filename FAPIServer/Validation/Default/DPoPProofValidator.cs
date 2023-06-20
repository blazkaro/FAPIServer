using Base64Url;
using FAPIServer.Extensions;
using FAPIServer.Helpers;
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
    public TokenValidationResult Validate(string dpop, DPoPValidationParameters parameters)
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

        var payload = validationResult.Paseto.Payload;
        if (!payload.HasTokenIdentifier())
            return TokenValidationResult.MissingClaim(PasetoRegisteredClaimNames.TokenIdentifier);

        var htm = (string?)payload["htm"];
        var htu = (string?)payload["htu"];

        if (htm.IsNullOrEmpty()) return TokenValidationResult.MissingClaim("htm");
        if (htu.IsNullOrEmpty()) return TokenValidationResult.MissingClaim("htu");

        if (htm.Equals(parameters.ValidHtm, StringComparison.OrdinalIgnoreCase)) return TokenValidationResult.InvalidClaim("htm");
        if (!htu.Equals(parameters.ValidHtu.ToString(), StringComparison.OrdinalIgnoreCase)) return TokenValidationResult.InvalidClaim("htu");

        if (parameters.ValidAth is not null)
        {
            var ath = (string?)payload["ath"];
            if (ath.IsNullOrEmpty()) return TokenValidationResult.MissingClaim("ath");

            if (ath != Base64UrlEncoder.Encode(parameters.ValidAth))
                return TokenValidationResult.InvalidClaim("ath");
        }

        return new(payload);
    }
}
