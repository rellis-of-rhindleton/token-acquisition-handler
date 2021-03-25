# TokenAcquisitionHandler

Simplistic example of using 
[MSAL-Angular's protected resource map approach]
(https://github.com/AzureAD/microsoft-authentication-library-for-js/blob/dev/lib/msal-angular/docs/v2-docs/initialization.md#get-tokens-for-web-api-calls)
with the [Microsoft.Identity.Web](https://github.com/AzureAD/microsoft-identity-web/wiki/web-apis) ITokenAcquisition service.

Useful if a web API needs to call multiple downstream APIs that need more customization than the IDownstreamWebApi approach allows.

Access tokens are retrieved by a DelegatingHandler.  

#### appsettings.json (or other configuration)

```json
"AzureAd": {
    "Instance": "https://login.microsoftonline.com/",
    "TenantId": "...",
    "ClientId": "...",
    "ClientSecret": "" // provide in env var or user secret
},
"DownstreamApiA": {
    "BaseUrl": "https://acmewidgets.com/api/systems/",
    "Scopes": "api://acmewidgets.com/.default",
    "AuthenticationFlow": "ClientCredentials"
},
"DownstreamApiB": {
    "BaseUrl": "https://acmewidgets.com/api/userprefs/",
    "Scopes": "api://acmewidgets.com/scope1 api://acmewidgets.com/scope2 api://acmewidgets.com/scope3"
    // "AuthenticationFlow": "OnBehalfOf" // this is the default
}
```

#### Startup.cs / ConfigureServices

```csharp
services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(Configuration.GetSection("AzureAd"))
    .EnableTokenAcquisitionToCallDownstreamApi()
    .AddInMemoryTokenCaches();

services.AddTokenAcquisitionHandler(
    Configuration.GetSection("DownstreamApiA"),
    Configuration.GetSection("DownstreamApiB")
);

services.AddHttpClient<DownstreamApiA>(client =>
    {
        // configure the client for this API
    })
    .AddHttpMessageHandler<TokenAcquisitionHandler>();

services.AddHttpClient<DownstreamApiB>(client =>
    {
        // configure the client for this API
    })
    .AddHttpMessageHandler<TokenAcquisitionHandler>();
```


