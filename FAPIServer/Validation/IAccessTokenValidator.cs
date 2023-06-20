using FAPIServer.Validation.Results;

namespace FAPIServer.Validation;

public interface IAccessTokenValidator
{
    Task<TokenValidationResult> ValidateAsync(string validIssuer, string accessToken, CancellationToken cancellationToken = default);
}
