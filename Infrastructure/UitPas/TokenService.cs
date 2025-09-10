using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.UitPas;

public class TokenService
{
    private readonly HttpClient _httpClient;
    private readonly string _clientId; 
    private readonly string _clientSecret;
    private readonly IConfiguration _configuration;
    private string? _accessToken;
    private DateTime _expiresAt;

    public TokenService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _clientId = _configuration["UitPas:ClientId"] ?? throw new ArgumentNullException(nameof(_clientId));
        _clientSecret = _configuration["UitPas:ClientSecret"] ?? throw new ArgumentNullException(nameof(_clientSecret));
    }

    public async Task<string?> GetAccessToken()
    {
        if (_accessToken is null || DateTime.UtcNow >= _expiresAt)
        {
            var body = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "grant_type", "client_credentials" },
                { "client_id", _clientId },
                { "client_secret", _clientSecret }
            });

            var response = await _httpClient.PostAsync(_configuration["Uitpas:TokenUrl"], body);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadFromJsonAsync<JsonElement>();
            _accessToken = json.GetProperty("access_token").GetString();

            if (_accessToken is null)
                return null;

            var expiresIn = json.GetProperty("expires_in").GetInt32();
            _expiresAt = DateTime.UtcNow.AddSeconds(expiresIn - 30);
        }
        return _accessToken;
    }
}

