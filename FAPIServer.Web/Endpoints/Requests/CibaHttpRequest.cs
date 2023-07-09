using FAPIServer.RequestHandling.Requests;
using Microsoft.AspNetCore.Mvc;

namespace FAPIServer.Web.Endpoints.Requests;

public class CibaHttpRequest : CibaRequest
{
    [FromForm(Name = "authorization_details")]
    public override string AuthorizationDetails { get => base.AuthorizationDetails; set => base.AuthorizationDetails = value; }

    [FromForm(Name = "claims")]
    public override string Claims { get => base.Claims; set => base.Claims = value; }

    [FromForm(Name = "client_notification_token")]
    public override string? ClientNotificationToken { get => base.ClientNotificationToken; set => base.ClientNotificationToken = value; }

    [FromForm(Name = "id_token_hint")]
    public override string? IdTokenHint { get => base.IdTokenHint; set => base.IdTokenHint = value; }

    [FromForm(Name = "login_hint")]
    public override string? LoginHint { get => base.LoginHint; set => base.LoginHint = value; }

    [FromForm(Name = "binding_message")]
    public override string? BindingMessage { get => base.BindingMessage; set => base.BindingMessage = value; }

    [FromForm(Name = "user_code")]
    public override string? UserCode { get => base.UserCode; set => base.UserCode = value; }

    [FromForm(Name = "grant_id")]
    public override string? GrantId { get => base.GrantId; set => base.GrantId = value; }

    [FromForm(Name = "grant_management_action")]
    public override string? GrantManagementAction { get => base.GrantManagementAction; set => base.GrantManagementAction = value; }

    [FromForm(Name = "dpop_pkh")]
    public override string? DPoPPkh { get => base.DPoPPkh; set => base.DPoPPkh = value; }
}
