﻿using FAPIServer.RequestHandling.Requests;
using FAPIServer.Storage.Models;

namespace FAPIServer.Validation.Models;

public class ValidatedTokenRequest : ValidatedRequest<TokenRequest>
{
    public ValidatedTokenRequest(TokenRequest rawRequest, Client client)
        : base(rawRequest, client)
    {
    }

    public ValidatedTokenRequest(TokenRequest rawRequest, Client client, AuthorizationCode? authorizationCode)
        : base(rawRequest, client)
    {
        AuthorizationCode = authorizationCode;
    }

    public ValidatedTokenRequest(TokenRequest rawRequest, Client client, RefreshToken? refreshToken)
        : base(rawRequest, client)
    {
        RefreshToken = refreshToken;
    }

    public ValidatedTokenRequest(TokenRequest rawRequest, Client client, CibaObject? cibaObject)
        : base(rawRequest, client)
    {
        CibaObject = cibaObject;
    }

    public AuthorizationCode? AuthorizationCode { get; init; }
    public RefreshToken? RefreshToken { get; init; }
    public CibaObject? CibaObject { get; init; }
}
