﻿using System.Text.Json.Serialization;

namespace FAPIServer.Models;

public class IdTokenPayload : TokenPayloadBase<IdTokenPayload>
{
    [JsonPropertyName("iss")]
    public string Issuer { get; set; }

    [JsonPropertyName("aud")]
    public string Audience { get; set; }

    [JsonPropertyName("sub")]
    public string Subject { get; set; }

    [JsonPropertyName("nbf")]
    public DateTime NotBefore { get; set; }

    [JsonPropertyName("exp")]
    public DateTime Expiration { get; set; }

    [JsonPropertyName("nonce")]
    public string? Nonce { get; set; }

    [JsonPropertyName("auth_time")]
    public DateTime? AuthTime { get; set; }
}
