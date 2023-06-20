using Base64Url;

namespace FAPIServer.Helpers;

public static class PaserkHelper
{
    public static bool IsK4Public(string paserk)
    {
        var segments = paserk.Split('.');
        return segments.Length == 3
            && segments[0] == "k4"
            && segments[1] == "public"
            && Base64UrlEncoder.Validate(segments[2], out int bytesWritten)
            && bytesWritten == 32;
    }
}
