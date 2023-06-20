using FAPIServer.Storage.ValueObjects;

namespace FAPIServer.ResponseHandling.Models;

public class GrantQueryingResponse
{
    public IEnumerable<AuthorizationDetail>? AuthorizationDetails { get; set; }
    public IEnumerable<string>? Claims { get; set; }
}
