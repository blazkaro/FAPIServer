using FAPIServer.Storage.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace FAPIServer.Storage.EntityFramework.Entities.Converters;

public class LifetimeConverter : ValueConverter<Lifetime, int>
{
    public LifetimeConverter()
        : base(
            v => v.Seconds,
            v => new Lifetime(v))
    {

    }
}