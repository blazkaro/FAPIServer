using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace FAPIServer.Storage.EntityFramework.Entities.Converters;

public class UriConverter : ValueConverter<Uri, string>
{
    public UriConverter()
        : base(
            v => v.ToString(),
            v => new Uri(v))
    {

    }
}
