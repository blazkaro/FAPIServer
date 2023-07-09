using FAPIServer.ResponseHandling.Models;
using FAPIServer.Validation.Models;

namespace FAPIServer.ResponseHandling;

public interface ICibaResponseGenerator
{
    Task<CibaResponse> GenerateAsync(ValidatedCibaRequest validatedRequest, CancellationToken cancellationToken = default);
}
