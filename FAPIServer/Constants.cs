namespace FAPIServer;

public static class Constants
{
    public static class ClientAssertionTypes
    {
        public const string PasetoBearer = "urn:ietf:params:oauth:client-assertion-type:paseto-bearer";
    }

    public static class ClientAuthenticationMethods
    {
        public const string PrivateKeyPaseto = "private_key_paseto";
    }

    public static class GrantTypes
    {
        public const string AuthorizationCode = "authorization_code";
        public const string ClientCredentials = "client_credentials";
        public const string RefreshToken = "refresh_token";
        public const string Ciba = "urn:openid:params:grant-type:ciba";

        public static readonly string[] Types = { AuthorizationCode, ClientCredentials, RefreshToken, Ciba };
    }

    public static class BuiltInClaims
    {
        public const string Subject = "sub";
    }

    public static class BuiltInAuthorizationDetails
    {
        public static class OpenId
        {
            public const string Type = "openid";
            public static class Actions
            {
                public const string GrantManagementQuery = "grant_management_query";
                public const string GrantManagementRevoke = "grant_management_revoke";
                public const string OfflineAccess = "offline_access";
            };
        }
    }

    public const string RequestUriUrn = "urn:ietf:params:oauth:request_uri:";

    public static class CodeChallengeMethods
    {
        public const string S256 = "S256";
    }

    public static class AccessTokenTypes
    {
        public const string DPoP = "DPoP";
    }

    public static class GrantManagementActions
    {
        public const string Create = "create";
        public const string Merge = "merge";
        public const string Replace = "replace";

        public static readonly string[] Actions = { Create, Merge, Replace };
    }

    public static class PromptTypes
    {
        public const string Login = "login";

        public static readonly string[] Types = { Login };
    }

    public static class CibaModes
    {
        public const string Poll = "poll";
        public const string Ping = "ping";
    }

    public const string CibaNotificationHttpClientName = "CIBA-CLIENT-NOTIFICATION";
}
