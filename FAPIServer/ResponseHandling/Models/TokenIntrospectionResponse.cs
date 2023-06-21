using FAPIServer.Models;
using FAPIServer.Storage.ValueObjects;

namespace FAPIServer.ResponseHandling.Models;

public class TokenIntrospectionResponse
{
    public bool Active { get; set; }
    public string? ClientId { get; set; }
    public string? TokenType { get; set; }
    public DateTime? NotBefore { get; set; }
    public DateTime? Expiration { get; set; }
    public string? Sub { get; set; }
    public CnfClaim? Cnf { get; set; }
    public IEnumerable<AuthorizationDetail>? AuthorizationDetails { get; set; }
    public string? Jti { get; set; }
}
