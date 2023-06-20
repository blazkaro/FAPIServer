using FAPIServer.Validation.Contexts;
using FAPIServer.Validation.Results;

namespace FAPIServer.Validation;

public interface IDPoPProofValidator
{
    TokenValidationResult Validate(string dpop, DPoPValidationParameters parameters);
}
