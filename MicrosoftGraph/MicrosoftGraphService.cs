using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace WebApi.MicrosoftGraph
{
    public class MicrosoftGraphService
    {
        private static readonly string _userFieldNames = Uri.EscapeDataString(User.FieldNames);

        private readonly HttpClient _httpClient;
        private readonly ILogger<MicrosoftGraphService> _logger;

        public MicrosoftGraphService(HttpClient httpClient, ILogger<MicrosoftGraphService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<User> CurrentUserAsync()
        {
            _logger.LogDebug("Retrieving current user information from Graph");

            var json = await _httpClient.GetStringAsync($"me?$select={_userFieldNames}");

            var user = JsonSerializer.Deserialize<User>(json);
            if (user != null) user.QueryDateTimeUtc = DateTime.UtcNow;
            return user;
        }

    }
}
