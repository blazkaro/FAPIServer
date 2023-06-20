namespace FAPIServer.Storage.ValueObjects;

public record Ed25519Key
{
    public Ed25519Key(byte[] key)
    {
        if (!CanCreate(key))
            throw new ArgumentException($"The given key is not valid ed25519 key.", nameof(key));

        Value = key;
    }
    
    public byte[] Value { get; init; }

    public static bool CanCreate(byte[] key) => key is not null && key.Length == 32;
}
