using FAPIServer.RequestHandling.Requests;
using System.Web;

namespace FAPIServer.Helpers;

public static class InteractionHelper
{
    public static AuthorizationRequest GetAuthorizationContextFromReturnUrl(string returnUrl)
    {
        var query = HttpUtility.ParseQueryString(new string(returnUrl.SkipWhile(p => p != '?').ToArray()));
        return new AuthorizationRequest
        {
            ClientId = query["client_id"],
            RequestUri = query["request_uri"]
        };
    }

    public static string RemoveRequestUriPrefix(string requestUri) => requestUri.Replace(Constants.RequestUriUrn, string.Empty);
}
