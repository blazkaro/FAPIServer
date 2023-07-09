using FAPIServer.Extensions;
using FAPIServer.Models;
using FAPIServer.Services;

namespace FAPIServer.RequestHandling.Default;

public class UserInfoHandler : IUserInfoHandler
{
    private readonly IUserService _userService;

    public UserInfoHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<IDictionary<string, object>> HandleAsync(AccessTokenPayload atPayload, CancellationToken cancellationToken = default)
    {
        if (atPayload is null)
            throw new ArgumentNullException(nameof(atPayload));

        if (atPayload.Subject == atPayload.ClientId)
            throw new InvalidOperationException("Invalid token. The 'sub' claim does not identify user");

        var requestedClaims = atPayload.Claims?.FromSpaceDelimitedString();
        var claims = requestedClaims is not null
            ? await _userService.GetClaimsAsync(atPayload.Subject, requestedClaims, cancellationToken)
            : new Dictionary<string, object>();

        return claims;
    }
}
