using FAPIServer.Models;
using FAPIServer.Validation.Results;

namespace FAPIServer.Validation;

public interface IIdTokenValidator
{
    Task<TokenValidationResult<IdTokenPayload>> ValidateAsync(string idToken, string validIssuer, string validAudience, CancellationToken cancellationToken = default);
}
