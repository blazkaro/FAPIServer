using FAPIServer.Storage.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace FAPIServer.Storage.EntityFramework.Entities.Converters;

public class Ed25519KeyConverter : ValueConverter<Ed25519Key, string>
{
    public Ed25519KeyConverter()
        : base(
            v => Convert.ToBase64String(v.Value),
            v => new Ed25519Key(Convert.FromBase64String(v)))
    {

    }
}
