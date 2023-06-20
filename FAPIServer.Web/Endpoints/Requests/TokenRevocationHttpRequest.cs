using FAPIServer.RequestHandling.Requests;
using Microsoft.AspNetCore.Mvc;

namespace FAPIServer.Web.Controllers.Requests;

public class TokenRevocationHttpRequest : TokenRevocationRequest
{
    [FromForm(Name = "token")]
    public override string Token { get => base.Token; set => base.Token = value; }
}
