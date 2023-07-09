namespace FAPIServer;

public enum Error
{
    InvalidClient,
    UnauthorizedClient,
    InvalidRequest,
    InvalidGrantId,
    InvalidClaims,
    InvalidRedirectUri,
    InvalidAuthorizationDetails,
    InvalidAuthorizationDetaiTypes,
    InvalidRequestUri,
    UnsupportedGrantType,
    InvalidGrant,
    InvalidDPoPProof,
    UnknownUserId,
    MissingUserCode,
    InvalidUserCode,
    InvalidBindingMessage,
    AuthorizationPending,
    ExpiredToken,
    AccessDenied
}

public static class ErrorDescriptions
{
    public static string MissingParameter(string paramName) => $"The required parameter '{paramName}' is missing";
    public const string UnauthorizedClient = "The client is not allowed to use requested grant type";
    public static string LengthRestrictionsUnsatisfied(string paramName) => $"The '{paramName}' does not meet the length restrictions";
    public static string NotSupportedValue(string value, string paramName) => $"The '{value}' is not supported value for '{paramName}'";
}