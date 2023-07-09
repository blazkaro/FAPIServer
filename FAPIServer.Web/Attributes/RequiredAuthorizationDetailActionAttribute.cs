using FAPIServer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FAPIServer.Web.Attributes;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class RequiredAuthorizationDetailActionAttribute : Attribute, IAuthorizationFilter
{
    private readonly string _requiredType;
    private readonly IEnumerable<string> _requiredActions;

    public RequiredAuthorizationDetailActionAttribute(string requiredType, params string[] requiredActions)
    {
        _requiredType = requiredType ?? throw new ArgumentNullException(nameof(requiredType));
        _requiredActions = requiredActions;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var claimsPrincipal = context.HttpContext.User;
        if (claimsPrincipal.Identity is null || !claimsPrincipal.Identity.IsAuthenticated || !claimsPrincipal.HasClaim(p => p.Type == "at_payload"))
        {
            InsufficentScope(context);
            return;
        }

        var accessToken = AccessTokenPayload.FromJson(claimsPrincipal.Claims.SingleOrDefault(p => p.Type == "at_payload")!.Value);
        if (accessToken is null)
        {
            InsufficentScope(context);
            return;
        }

        var authorizationDetail = accessToken.AuthorizationDetails?.SingleOrDefault(p => p.Type == _requiredType);
        if (authorizationDetail is null || !_requiredActions.All(authorizationDetail.Actions.ContainsKey))
        {
            InsufficentScope(context);
        }
    }

    private sealed class InsufficientScopeActionResult : IActionResult
    {
        public async Task ExecuteResultAsync(ActionContext context)
        {
            context.HttpContext.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.HttpContext.Response.WriteAsJsonAsync(new
            {
                error = "insufficient_scope",
                error_description = "The provided access token has insufficient scope to access this resource"
            });
        }
    }

    private static void InsufficentScope(AuthorizationFilterContext context)
    {
        context.Result = new InsufficientScopeActionResult();
    }
}
