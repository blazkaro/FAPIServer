using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace FAPIServer.Storage.EntityFramework.Entities.Converters;

public class StringCollectionConverter : ValueConverter<ICollection<string>, string>
{
    private static ICollection<string> FromSpaceDelimited(string s) => s.Split(' ').ToList();

    public StringCollectionConverter()
        : base(
            v => string.Join(' ', v),
            v => FromSpaceDelimited(v))
    {

    }
}
