using FAPIServer.Validation.Models;
using System.Security.Claims;

namespace FAPIServer.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static string? GetSubject(this ClaimsPrincipal user)
    {
        return user.Claims.SingleOrDefault(p => p.Type == "sub")?.Value;
    }

    public static DateTime? GetAuthTime(this ClaimsPrincipal user)
    {
        return DateTime.TryParse(user.Claims.SingleOrDefault(p => p.Type == "auth_time")?.Value, out DateTime authTime) ? authTime : null;
    }

    public static ValidatedUser ToValidatedUser(this ClaimsPrincipal user)
    {
        var subject = user.GetSubject();
        var authTime = user.GetAuthTime();
        if (subject.IsNullOrEmpty() || !authTime.HasValue)
            throw new InvalidOperationException($"The provided {nameof(ClaimsPrincipal)} is not valid user");

        return new ValidatedUser(subject, authTime.Value, user);
    }
}
