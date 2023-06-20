namespace FAPIServer.Storage.ValueObjects;

public record Lifetime
{
    public Lifetime(int seconds)
    {
        if (seconds < 0)
            throw new ArgumentException($"`{nameof(seconds)}` must be greater or equal to 0", nameof(seconds));

        Seconds = seconds;
    }

    public int Seconds { get; init; }

    public double GetMinutes() => Seconds / 60;
    public double GetHours() => Seconds / 3600;
}
