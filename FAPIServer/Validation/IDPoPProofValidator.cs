using FAPIServer.Models;
using FAPIServer.Validation.Contexts;
using FAPIServer.Validation.Results;

namespace FAPIServer.Validation;

public interface IDPoPProofValidator
{
    TokenValidationResult<DPoPPayload> Validate(string dpop, DPoPValidationParameters parameters);
}
