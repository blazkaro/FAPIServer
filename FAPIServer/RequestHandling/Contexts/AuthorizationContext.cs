using FAPIServer.Extensions;
using FAPIServer.RequestHandling.Requests;
using FAPIServer.Validation.Models;
using System.Security.Claims;

namespace FAPIServer.RequestHandling.Contexts;

public class AuthorizationContext
{
    public AuthorizationContext(AuthorizationRequest request, string responseIssuer, ClaimsPrincipal? user)
    {
        if (string.IsNullOrEmpty(responseIssuer))
            throw new ArgumentException($"'{nameof(responseIssuer)}' cannot be null or empty.", nameof(responseIssuer));

        Request = request ?? throw new ArgumentNullException(nameof(request));
        ResponseIssuer = responseIssuer;
        User = user;
    }

    public AuthorizationRequest Request { get; set; }
    public string ResponseIssuer { get; set; }
    public ClaimsPrincipal? User { get; set; }

    private ValidatedUser _validatedUser { get; set; }

    public void SetValidUser()
    {
        var subject = User?.GetSubject();
        var authTime = User?.GetAuthTime();
        if (User is null || subject.IsNullOrEmpty() || !authTime.HasValue)
            return;

        _validatedUser = new ValidatedUser(subject, authTime.Value, User);
    }

    public ValidatedUser GetValidUser()
    {
        if (_validatedUser is null)
            throw new InvalidOperationException("There is no valid user in this context");

        return _validatedUser;
    }
}
