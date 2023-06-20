using Base64Url;

namespace FAPIServer.Storage.ValueObjects;

public record Base64UrlEncodedString
{
    public Base64UrlEncodedString(string s)
    {
        if (!CanCreate(s))
            throw new ArgumentException($"The '{s}' is not valid Base64-Url encoded string", nameof(s));

        Value = s;
    }

    public string Value { get; init; }

    public static bool CanCreate(string s) => Base64UrlEncoder.Validate(s, out _);

    public byte[] Decode() => Base64UrlEncoder.Decode(Value);
}
