using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace FAPIServer.Storage.EntityFramework.Entities.Converters;

public class StringEnumerableConverter : ValueConverter<IEnumerable<string>, string>
{
    private static IEnumerable<string> FromSpaceDelimited(string s) => s.Split(' ');

    public StringEnumerableConverter()
        : base(
            v => string.Join(' ', v),
            v => FromSpaceDelimited(v))
    {

    }
}
