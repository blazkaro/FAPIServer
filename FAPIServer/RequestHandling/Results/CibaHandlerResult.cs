using FAPIServer.ResponseHandling.Models;

namespace FAPIServer.RequestHandling.Results;

public class CibaHandlerResult : HandlerResultBase
{
    public CibaHandlerResult(Error? error) : base(error)
    {
    }

    public CibaHandlerResult(Error? error, string? failureMessage) : base(error, failureMessage)
    {
    }

    public CibaHandlerResult(CibaResponse cibaResponse)
    {
        Success = true;
        CibaResponse = cibaResponse;
    }

    public CibaResponse CibaResponse { get; init; }
}
