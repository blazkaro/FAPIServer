namespace FAPIServer.Services.Models;

public class UserCibaContext
{
    public UserCibaContext(bool requireUserCode, bool isUserCodeValid)
    {
        RequireUserCode = requireUserCode;
        IsUserCodeValid = isUserCodeValid;
    }

    public bool RequireUserCode { get; init; }
    public bool IsUserCodeValid { get; init; }
}
