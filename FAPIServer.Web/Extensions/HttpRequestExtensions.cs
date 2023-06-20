using Microsoft.AspNetCore.Http;

namespace FAPIServer.Web.Extensions;

public static class HttpRequestExtensions
{
    public static Uri GetSchemeAndHost(this HttpRequest request) => new Uri($"{request.Scheme}://{request.Host}");
    public static Uri GetRequestedEndpointUri(this HttpRequest request) => new Uri($"{request.GetSchemeAndHost().ToString().TrimEnd('/')}{request.Path}");
}
