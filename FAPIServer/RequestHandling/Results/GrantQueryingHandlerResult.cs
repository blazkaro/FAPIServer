using FAPIServer.ResponseHandling.Models;

namespace FAPIServer.RequestHandling.Results;

public class GrantQueryingHandlerResult
{
    public GrantQueryingHandlerResult()
    {
        
    }

    public GrantQueryingHandlerResult(GrantQueryingResponse response)
    {
        Success = true;
        Response = response ?? throw new ArgumentNullException(nameof(response));
    }

    public bool Success { get; init; }
    public GrantQueryingResponse Response { get; init; }
}
