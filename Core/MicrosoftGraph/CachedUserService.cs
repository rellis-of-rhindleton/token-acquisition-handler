using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace WebApi.Core.MicrosoftGraph
{
    /// <summary>
    /// Wraps MicrosoftGraphService to cache current user info.
    /// </summary>
    public class CachedUserService
    {
        private static readonly TimeSpan _cacheTime = TimeSpan.FromMinutes(60);

        private readonly ILogger<MicrosoftGraphService> _logger;
        private readonly IMemoryCache _cache;
        private readonly MicrosoftGraphService _graph;

        public CachedUserService(IMemoryCache cache, MicrosoftGraphService graph, ILogger<MicrosoftGraphService> logger)
        {
            _cache = cache;
            _graph = graph;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves current user information from Microsoft Graph.
        /// Caches it for up to an hour.
        /// </summary>
        public async Task<User> CurrentUserAsync(ClaimsPrincipal principal, bool forceRefresh = false)
        {
            if (principal == null) throw new ArgumentNullException(nameof(principal));
            if (principal.Identity?.IsAuthenticated != true) throw new ArgumentException("User is not authenticated.");

            var key = "graph_user_" + principal.Identity.Name;

            if (!forceRefresh && _cache.TryGetValue(key, out User user))
            {
                _logger.LogTrace("Retrieved user from cache.");
                return user;
            }

            _logger.LogDebug("Fetching user information from Graph");
            user = await _graph.CurrentUserAsync();

            _cache.Set(key, user, _cacheTime);

            return user;
        }

    }
}
