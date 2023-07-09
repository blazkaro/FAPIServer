using FAPIServer.Storage.ValueObjects;

namespace FAPIServer;

public class FapiOptions
{
    public bool UseClientAssertionRevocation { get; set; } = true;
    public Lifetime AuthorizationResponseLifetime { get; set; } = new Lifetime(10);
    public bool AlwaysRequireInteractionWhenMergingGrant { get; set; } = false;
    public InputRestrictions InputRestrictions { get; set; } = new();
    public bool GrantManagementActionRequired { get; set; } = false;
    public Lifetime CibaInterval { get; set; } = new Lifetime(5);
}

public class InputRestrictions
{
    public short MinStateLength { get; set; } = 43;
    public short MaxStateLength { get; set; } = 384;
    public short MinNonceLength { get; set; } = 43;
    public short MaxNonceLength { get; set; } = 384;
    public byte MinCodeChallengeLength { get; set; } = 43;
    public byte MaxCodeChallengeLength { get; set; } = 128;
    public short MaxCibaClientNotificationTokenLength { get; set; } = 1024;
    public byte MaxCibaBindingMessageLength { get; set; } = 8;
}