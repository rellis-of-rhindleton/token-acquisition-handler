using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Identity.Web;

namespace WebApi.Core.TokenAcquisition
{
    /// <summary>
    /// Acquires access tokens and adds them as Authorization headers to outgoing requests.
    /// </summary>
    public class TokenAcquisitionHandler : DelegatingHandler
    {
        private readonly ITokenAcquisition _tokenProvider;
        private readonly ProtectedResourceList _protectedResources;

        public TokenAcquisitionHandler(ITokenAcquisition tokenProvider, ProtectedResourceList protectedResources)
        {
            _tokenProvider = tokenProvider;
            _protectedResources = protectedResources;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var protectedResource = _protectedResources.Find(request.RequestUri);
            if (protectedResource != null && protectedResource.Scopes.Length > 0)
            {
                var token = await protectedResource.AcquireTokenAsync.Invoke(_tokenProvider);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return await base.SendAsync(request, cancellationToken);
        }

    }
}
