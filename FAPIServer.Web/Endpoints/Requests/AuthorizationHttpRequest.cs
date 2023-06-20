using FAPIServer.RequestHandling.Requests;
using Microsoft.AspNetCore.Mvc;

namespace FAPIServer.Web.Controllers.Requests;

public class AuthorizationHttpRequest : AuthorizationRequest
{
    [FromQuery(Name = "client_id")]
    public override string ClientId { get => base.ClientId; set => base.ClientId = value; }

    [FromQuery(Name = "request_uri")]
    public override string RequestUri { get => base.RequestUri; set => base.RequestUri = value; }
}
