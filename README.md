# FAPI Server
That's flexible, but not fully compatible implementation of <b>FAPI, OAuth2.1 and OpenID Connect</b> specifications. <b>It supports</b>:
- [Grant Management for OAuth2.0](https://openid.net/specs/fapi-grant-management.html)
- [OAuth 2.0 Rich Authorization Requests](https://datatracker.ietf.org/doc/html/rfc9396) in modified version, will explain later
- [OAuth 2.0 Pushed Authorization Requests](https://www.rfc-editor.org/rfc/rfc9126.html)
- [OAuth 2.0 Demonstrating Proof-of-Possession at the Application Layer (DPoP)](https://datatracker.ietf.org/doc/html/draft-ietf-oauth-dpop)
- Authorization endpoint
- Token endpoint
- Token revocation endpoint
- Token introspection endpoint
- User info endpoint
- Discovery endpoint

#### Why it's not fully compatible?
- It uses PASETO instead of JWT, and PASERK instead of JWK. Every token like access token, authorization response and DPoP uses PASETO.
- It doesn't support mTLS, only private_key_paseto (private_key_jwt that uses PASETO) as authentication method
- Currently, it doesn't support [Client Initiated Backchannel Authentication](https://openid.net/specs/openid-client-initiated-backchannel-authentication-core-1_0.html) and logout mechanisms, neither front and backchannel logout.
- Currently, it doesn't support server-provided nonce defined in [FAPI 2.0 Security Profile](https://openid.net/specs/fapi-2_0-security-profile.html)
#### Difference between RAR specification and this implementation
See official example of authorization_details:
```json
{
   "type":"photo-api",
   "actions":[
      "read",
      "write"
   ],
   "locations":[
      "https://server.example.net/",
      "https://resource.local/other"
   ],
   "datatypes":[
      "metadata",
      "images"
   ],
   "geolocation":[
      {
         "lat":-32.364,
         "lng":153.207
      },
      {
         "lat":-35.364,
         "lng":158.207
      }
   ]
}
```
The resource server won't know whether the `read` access if for both datatypes `metadata` and `images`, or only for one of them. The same problem is with `write` permission. What's more, it's not clear whether the permissions are both for both geolocations or there is `read` for first geolocation and `write` for second, or in reverse order. It's completely not clear.
Now imagine you want to use `Grant Management` and `merge` some `authorization_details` to existing. I think there is a lot of ambiguity here, so I decided to design my own format for `authorization_details`. The above example interpretation (because there can be many) could look as below:
```json
{
   "type":"photo-api",
   "actions":{
      "read":{
         "geolocation":[
            {
               "lat":-32.364,
               "lng":153.207
            },
            {
               "lat":-35.364,
               "lng":158.207
            }
         ],
         "metadata":[
            "title",
            "description",
            "author",
            "likes",
            "created_at"
         ]
      },
      "write":{
         "geolocation":{
            "lat":-35.364,
            "lng":158.207
         }
      }
   },
   "locations":[
      "https://server.example.net/",
      "https://resource.local/other"
   ]
}
```
The "description" is specific to every action. This approach provides high accuracy and allows to flexibly express your needs.

That's example of `authorization_details` that doesn't need to be descriptive, so used as `scope` (which is not supported) alternative:
```json
{
   "type":"openid",
   "actions":{
      "offline_access":{
         
      },
      "grant_management_query":{
         
      },
      "grant_management_revoke":{
         
      }
   },
   "locations":[
      
   ]
}
```
While the `openid` is special, built-in type, it doesn't need to have specified locations. It will be processed by authorization server only.

## Roadmap
- [ ] Support for CIBA
- [ ] Support for application-level request signing between client and authorization server by using `client_assertion` and `DPoP` mechanism.
- [ ] Support for encrypting userinfo and token introspection responses by using ECDH with Ephermal Keys.
- [ ] Support for DPoP revocation after use (the same mechanism as with `client_assertion`)
- [ ] Add optional, configurable application-level authorization server`s response body security by using signatures.
- [ ] Add better documentation for project
- [ ] Add unit tests

## Usage
- Create ASP NET project and download `FAPIServer.Web` package.
- Then, if you don't want to use custom, download `FAPIServer.Storage.EntityFramework`
- Then configure services:
```csharp
// Configure storage services
services.AddFapiStorageEntityFramework();

// Configure FAPI Server
services.AddFapiServer()
    .UseInMemorySecretCredentialsStore()
    .AddWebServices()
    .UseDefaultAuthorizationRequestPersistenceService()
    .AddUserClaimsService<ClaimsServiceFake>();
 ```
You need to create your custom `IUserClaimsService` to use at userinfo endpoint.

<i>Remember to call app.MapControllers()</i>

### Short explanation of important implementation specific aspects
You can use in memory credentials store, as shown below, but then singleton with ed25519 key-pair will be registered. Consider your own implementation in production.

The default authorization request persistence service will use `IParObjectStore` to manage authorization sessions. It will create `sid`, store it in cookie and associate with appropriate `ParObject`. If you want different behavior, you also can use your own implementation.

If you don't specify `dpop_pkh` parameter in PAR request, or you use machine-to-machine grant and don't use `DPoP` header at token endpoint, the access token will be bounded to the client`s authentication public key.

### Security policy
If you see security issues, please contact me by email, blazkaro.programmer@protonmail.com
