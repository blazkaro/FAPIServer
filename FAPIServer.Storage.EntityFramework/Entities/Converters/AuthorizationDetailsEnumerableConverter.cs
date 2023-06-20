using FAPIServer.Storage.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;

namespace FAPIServer.Storage.EntityFramework.Entities.Converters;

public class AuthorizationDetailsEnumerableConverter : ValueConverter<IEnumerable<AuthorizationDetail>, string>
{
    // Method here to avoid problems with optional parameters
    private static string Serialize(IEnumerable<AuthorizationDetail> authorizationDetails) => JsonSerializer.Serialize(authorizationDetails);
    private static IEnumerable<AuthorizationDetail> Deserialize(string json)
        => JsonSerializer.Deserialize<IEnumerable<AuthorizationDetail>>(json) ?? Array.Empty<AuthorizationDetail>();

    public AuthorizationDetailsEnumerableConverter()
        : base(
            v => Serialize(v),
            v => Deserialize(v))
    {

    }
}
