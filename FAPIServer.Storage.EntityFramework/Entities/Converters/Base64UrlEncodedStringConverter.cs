using FAPIServer.Storage.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace FAPIServer.Storage.EntityFramework.Entities.Converters;

public class Base64UrlEncodedStringConverter : ValueConverter<Base64UrlEncodedString, string>
{
    public Base64UrlEncodedStringConverter()
        : base(
            v => v.Value,
            v => new Base64UrlEncodedString(v))
    {

    }
}