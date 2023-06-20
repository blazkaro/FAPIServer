using FAPIServer.Storage.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;

namespace FAPIServer.Storage.EntityFramework.Entities.Converters;

public class AuthorizationDetailsCollectionConverter : ValueConverter<ICollection<AuthorizationDetail>, string>
{
    // Method here to avoid problems with optional parameters
    private static string Serialize(ICollection<AuthorizationDetail> authorizationDetails) => JsonSerializer.Serialize(authorizationDetails);
    private static ICollection<AuthorizationDetail> Deserialize(string json)
        => JsonSerializer.Deserialize<ICollection<AuthorizationDetail>>(json) ?? new List<AuthorizationDetail>();

    public AuthorizationDetailsCollectionConverter()
        : base(
            v => Serialize(v),
            v => Deserialize(v))
    {

    }
}
