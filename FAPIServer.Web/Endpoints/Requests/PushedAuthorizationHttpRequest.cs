using FAPIServer.RequestHandling.Requests;
using Microsoft.AspNetCore.Mvc;

namespace FAPIServer.Web.Controllers.Requests;

public class PushedAuthorizationHttpRequest: PushedAuthorizationRequest
{
    [FromForm(Name = "authorization_details")]
    public override string? AuthorizationDetails { get => base.AuthorizationDetails; set => base.AuthorizationDetails = value; }

    [FromForm(Name = "claims")]
    public override string? Claims { get => base.Claims; set => base.Claims = value; }

    [FromForm(Name = "redirect_uri")]
    public override string RedirectUri { get => base.RedirectUri; set => base.RedirectUri = value; }

    [FromForm(Name = "state")]
    public override string State { get => base.State; set => base.State = value; }

    [FromForm(Name = "nonce")]
    public override string Nonce { get => base.Nonce; set => base.Nonce = value; }

    [FromForm(Name = "code_challenge_method")]
    public override string CodeChallengeMethod { get => base.CodeChallengeMethod; set => base.CodeChallengeMethod = value; }

    [FromForm(Name = "code_challenge")]
    public override string CodeChallenge { get => base.CodeChallenge; set => base.CodeChallenge = value; }

    [FromForm(Name = "grant_id")]
    public override string? GrantId { get => base.GrantId; set => base.GrantId = value; }

    [FromForm(Name = "grant_management_action")]
    public override string? GrantManagementAction { get => base.GrantManagementAction; set => base.GrantManagementAction = value; }

    [FromForm(Name = "dpop_pkh")]
    public override string? DPoPPkh { get => base.DPoPPkh; set => base.DPoPPkh = value; }

    [FromForm(Name = "prompt")]
    public override string? Prompt { get => base.Prompt; set => base.Prompt = value; }

    [FromForm(Name = "max_age")]
    public override int? MaxAge { get => base.MaxAge; set => base.MaxAge = value; }
}
