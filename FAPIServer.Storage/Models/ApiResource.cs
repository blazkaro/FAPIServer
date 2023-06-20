namespace FAPIServer.Storage.Models;

public class ApiResource
{
    /// <summary>
    /// Whether this API resource is enabled. Defaults to <c>true</c>
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// The URI of this API resource
    /// </summary>
    public Uri Uri { get; set; }

    /// <summary>
    /// The id of client which is registered as this API resource. It's required to authenticate API resources
    /// </summary>
    public string ClientId { get; set; }

    public IEnumerable<string> HandledAuthorizationDetailTypes { get; set; } = Array.Empty<string>();
}
