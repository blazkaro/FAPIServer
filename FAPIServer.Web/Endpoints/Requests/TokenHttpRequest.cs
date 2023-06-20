using FAPIServer.RequestHandling.Requests;
using Microsoft.AspNetCore.Mvc;

namespace FAPIServer.Web.Controllers.Requests;

public class TokenHttpRequest : TokenRequest
{
    [FromForm(Name = "grant_type")]
    public override string GrantType { get => base.GrantType; set => base.GrantType = value; }

    [FromForm(Name = "code")]
    public override string? Code { get => base.Code; set => base.Code = value; }

    [FromForm(Name = "code_verifier")]
    public override string? CodeVerifier { get => base.CodeVerifier; set => base.CodeVerifier = value; }

    [FromForm(Name = "redirect_uri")]
    public override string? RedirectUri { get => base.RedirectUri; set => base.RedirectUri = value; }

    [FromForm(Name = "authorization_details")]
    public override string? AuthorizationDetails { get => base.AuthorizationDetails; set => base.AuthorizationDetails = value; }

    [FromForm(Name = "authorization_details_types")]
    public override string? AuthorizationDetailsTypes { get => base.AuthorizationDetailsTypes; set => base.AuthorizationDetailsTypes = value; }

    [FromForm(Name = "claims")]
    public override string? Claims { get => base.Claims; set => base.Claims = value; }

    [FromForm(Name = "refresh_token")]
    public override string? RefreshToken { get => base.RefreshToken; set => base.RefreshToken = value; }

    [FromHeader(Name = "DPoP")]
    public override string? DPoP { get => base.DPoP; set => base.DPoP = value; }
}
