using FAPIServer.RequestHandling.Requests;
using Microsoft.AspNetCore.Mvc;

namespace FAPIServer.Web.Controllers.Requests;

public class ClientAuthHttpRequest : ClientAuthRequest
{
    [FromForm(Name = "client_id")]
    public override string ClientId { get => base.ClientId; set => base.ClientId = value; }

    [FromForm(Name = "client_assertion_type")]
    public override string ClientAssertionType { get => base.ClientAssertionType; set => base.ClientAssertionType = value; }

    [FromForm(Name = "client_assertion")]
    public override string ClientAssertion { get => base.ClientAssertion; set => base.ClientAssertion = value; }
}
