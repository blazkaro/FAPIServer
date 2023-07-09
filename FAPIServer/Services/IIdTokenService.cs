using FAPIServer.Storage.Models;
using FAPIServer.Storage.ValueObjects;

namespace FAPIServer.Services;

public interface IIdTokenService
{
    Task<string> GenerateAsync(string issuer, AuthorizationCode authorizationCode, Lifetime lifetime, CancellationToken cancellationToken = default);
    Task<string> GenerateAsync(string issuer, CibaObject cibaObject, Lifetime lifetime, CancellationToken cancellationToken = default);
}
