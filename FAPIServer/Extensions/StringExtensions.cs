using System.Diagnostics.CodeAnalysis;

namespace FAPIServer.Extensions;

public static class StringExtensions
{
    public static bool IsNullOrEmpty([NotNullWhen(false)] this string? s) => string.IsNullOrEmpty(s);
    public static IEnumerable<string> FromSpaceDelimitedString(this string s) => s.Split(' ');
    public static string ToSpaceDelimitedString(this IEnumerable<string> s) => string.Join(' ', s);
}
