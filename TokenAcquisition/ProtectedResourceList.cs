using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Identity.Web;

namespace WebApi.TokenAcquisition
{
    public class ProtectedResourceList
    {
        public List<ProtectedResource> Items { get; set; }

        public ProtectedResourceList()
        {
            Items = new List<ProtectedResource>();
        }

        public ProtectedResource Find(Uri requestUri)
        {
            foreach (var item in Items)
            {
                if (item.BaseUri.IsBaseOf(requestUri))
                    return item;
            }

            return null;
        }
    }

    public enum AuthenticationFlow
    {
        OnBehalfOf,
        ClientCredentials
    }

    public class ProtectedResourceOptions
    {
        public string BaseUrl { get; set; }
        public string Scopes { get; set; }
        public AuthenticationFlow? AuthenticationFlow { get; set; }
    }

    public class ProtectedResource
    {
        public Uri BaseUri { get; }
        public string[] Scopes { get; }
        public AuthenticationFlow AuthenticationFlow { get; }
        public Func<ITokenAcquisition, Task<string>> AcquireTokenAsync { get; }

        public ProtectedResource(ProtectedResourceOptions options)
        {
            BaseUri = new Uri(options.BaseUrl);
            Scopes = !string.IsNullOrEmpty(options.Scopes)
                ? options.Scopes.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                : Array.Empty<string>();
            AuthenticationFlow = options.AuthenticationFlow ?? AuthenticationFlow.OnBehalfOf;
            AcquireTokenAsync = CreateTokenCallback(AuthenticationFlow, Scopes);
        }

        public ProtectedResource(string baseUrl, string scopes, AuthenticationFlow authenticationFlow = AuthenticationFlow.OnBehalfOf)
        {
            if (string.IsNullOrEmpty(baseUrl)) throw new ArgumentException("Value cannot be null or empty.", nameof(baseUrl));

            BaseUri = new Uri(baseUrl);
            Scopes = !string.IsNullOrEmpty(scopes)
                ? scopes.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                : Array.Empty<string>();
            AuthenticationFlow = authenticationFlow;
            AcquireTokenAsync = CreateTokenCallback(AuthenticationFlow, Scopes);
        }

        public ProtectedResource(Uri baseUri, string[] scopes, AuthenticationFlow authenticationFlow = AuthenticationFlow.OnBehalfOf)
        {
            BaseUri = baseUri ?? throw new ArgumentNullException(nameof(baseUri));
            Scopes = scopes ?? throw new ArgumentNullException(nameof(scopes));
            AuthenticationFlow = authenticationFlow;
            AcquireTokenAsync = CreateTokenCallback(AuthenticationFlow, Scopes);
        }

        private static Func<ITokenAcquisition, Task<string>> CreateTokenCallback(AuthenticationFlow authenticationFlow, string[] scopes)
        {
            switch (authenticationFlow)
            {
                case AuthenticationFlow.ClientCredentials:
                    var scope = scopes.Single();
                    return tokenProvider => tokenProvider.GetAccessTokenForAppAsync(scope);

                case AuthenticationFlow.OnBehalfOf:
                    return tokenProvider => tokenProvider.GetAccessTokenForUserAsync(scopes);

                default:
                    throw new Exception($"AuthenticationFlow \"{authenticationFlow}\" not recognized.");
            }
        }

    }

}
