using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Core.MicrosoftGraph;
using WebApi.Core.TokenAcquisition;

namespace WebApi.Endpoints.Home
{
    [Route("")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly CachedUserService _cachedUserService;
        private readonly ProtectedResourceList _protectedResources;

        public HomeController(CachedUserService cachedUserService, ProtectedResourceList protectedResources)
        {
            _cachedUserService = cachedUserService;
            _protectedResources = protectedResources;
        }

        [HttpGet("")]
        [AllowAnonymous]
        public IActionResult Index()
        {
            var assemblyName = typeof(Program).Assembly.GetName();

            var result = new
            {
                name = assemblyName.Name,
                version = assemblyName.Version?.ToString(),
                user = User?.Identity?.Name
            };

            return Ok(result);
        }

        [HttpGet("protected-resources")]
        [AllowAnonymous]
        public IActionResult Config()
        {
            var content = _protectedResources.Items.Select(item =>
            new {
                baseUri = item.BaseUri,
                flow = item.AuthenticationFlow,
                scopes = item.Scopes
            });
            return Ok(content);
        }

        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            var identity = User.Identity;
            if (identity?.IsAuthenticated != true)
                return NoContent();

            var graphUser = await _cachedUserService.CurrentUserAsync(User);

            var tokenData = new
            {
                name = identity.Name,
                authType = identity.AuthenticationType,
                claims = User.Claims
                    .Select(c => new {c.Type, c.Value})
                    .OrderBy(c => c.Type)
                    .ThenBy(c => c.Value)
                    .ToArray()
            };

            var result = new
            {
                graphUser,
                tokenData
            };

            return Ok(result);
        }

    }
}
