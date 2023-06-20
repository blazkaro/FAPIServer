using Base64Url;

namespace FAPIServer.Helpers;

public static class PasetoHelper
{
    public static bool IsV4Public(string paseto)
    {
        var segments = paseto.Split('.');
        return (segments.Length == 3 || segments.Length == 4)
            && segments[0] == "v4"
            && segments[1] == "public"
            && Base64UrlEncoder.Validate(segments[2], out _)
            && (segments.Length != 4 || Base64UrlEncoder.Validate(segments[3], out _));
    }
}
