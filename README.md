# FAPI Server
That's flexible, but not fully compatible implementation of <b>FAPI, OAuth2.1 and OpenID Connect</b> specifications. <b>It supports</b>:
- [Grant Management for OAuth2.0](https://openid.net/specs/fapi-grant-management.html)
- [OAuth 2.0 Rich Authorization Requests](https://datatracker.ietf.org/doc/html/rfc9396) in modified version, will explain later
- [OAuth 2.0 Pushed Authorization Requests](https://www.rfc-editor.org/rfc/rfc9126.html)
- [OAuth 2.0 Demonstrating Proof-of-Possession at the Application Layer (DPoP)](https://datatracker.ietf.org/doc/html/draft-ietf-oauth-dpop)
- [OpenID Connect Client-Initiated Backchannel Authentication Flow - Core 1.0](https://openid.net/specs/openid-client-initiated-backchannel-authentication-core-1_0.html)
- Authorization endpoint
- Token endpoint
- Token revocation endpoint
- Token introspection endpoint
- User info endpoint
- Discovery endpoint

## Documentation
https://blazkaro.github.io/FAPIServerDocumentation/

#### Why it's not fully compatible?
- It uses PASETO instead of JWT, and PASERK instead of JWK. Every token like access token, authorization response and DPoP uses PASETO.
- It doesn't support mTLS, only private_key_paseto (private_key_jwt that uses PASETO) as authentication method
- Currently, it doesn't support server-provided nonce defined in [FAPI 2.0 Security Profile](https://openid.net/specs/fapi-2_0-security-profile.html)

## Roadmap
- [x] Support for CIBA
- [ ] Support for application-level request signing between client and authorization server by using `client_assertion` and `DPoP` mechanism, or by request objects. (milestone)
- [x] Support for signing userinfo, token introspection and grant querying responses
- [ ] Support for DPoP revocation after use (the same mechanism as with `client_assertion`)
- [ ] Add better documentation for project
- [ ] Add unit tests

### Security policy
If you see security issues, please contact me by email, blazkaro.programmer@protonmail.com
