
using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;

public class AuthService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;

    public AuthService(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        _config = config;
    }

    public async Task<string> GetTokenAsync()
    {
        Dictionary<string, string> credentials = new Dictionary<string, string>
        {
            ["client_id"] = GetRequiredConfig("Id"),
            ["client_secret"] = GetRequiredConfig("Secret"),
            ["grant_type"] = GetRequiredConfig("Grant_type"),
            ["scope"] = GetRequiredConfig("Scope")
        };

        string url = GetRequiredConfig("JwtUrl");

        using FormUrlEncodedContent content = new FormUrlEncodedContent(credentials);

        using HttpResponseMessage response = await _httpClient.PostAsync(url, content);

        response.EnsureSuccessStatusCode();

        AuthorizationResponse? result = await response.Content.ReadFromJsonAsync<AuthorizationResponse>();

        return result?.AccessToken ?? throw new InvalidOperationException("Authentication response did not contain an access token.");
    }

    private string GetRequiredConfig(string key)
    {
        return _config[$"Api:{key}"] ?? throw new InvalidOperationException($"Missing configuration: Api:{key}");
    }
}
