using FAPIServer.ResponseHandling.Models;
using FAPIServer.Validation.Models;

namespace FAPIServer.ResponseHandling;

public interface ITokenResponseGenerator
{
    Task<TokenResponse> GenerateAsync(ValidatedTokenRequest validatedRequest, string responseIssuer, CancellationToken cancellationToken = default);
}
