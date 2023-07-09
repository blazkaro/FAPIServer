namespace FAPIServer.ResponseHandling.Models;

public class CibaResponse
{
    public string AuthReqId { get; set; }
    public int ExpiresIn { get; set; }
    public int? Interval { get; set; }
}
