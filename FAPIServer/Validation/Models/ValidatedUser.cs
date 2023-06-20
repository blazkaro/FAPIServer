using System.Security.Claims;

namespace FAPIServer.Validation.Models;

public class ValidatedUser
{
    public ValidatedUser(string subject, DateTime authTime, ClaimsPrincipal user)
    {
        if (string.IsNullOrEmpty(subject))
            throw new ArgumentException($"'{nameof(subject)}' cannot be null or empty.", nameof(subject));

        if (authTime == default)
            throw new ArgumentException($"'{nameof(authTime)}' cannot be default of DateTime", nameof(authTime));

        Subject = subject;
        AuthTime = authTime;
        User = user ?? throw new ArgumentNullException(nameof(user));
    }

    public string Subject { get; set; }
    public DateTime AuthTime { get; set; }
    public ClaimsPrincipal User { get; set; }
}
