using FAPIServer.Models;
using FAPIServer.Validation.Results;

namespace FAPIServer.Validation;

public interface IAccessTokenValidator
{
    Task<TokenValidationResult<AccessTokenPayload>> ValidateAsync(string validIssuer, string accessToken, CancellationToken cancellationToken = default);
}
