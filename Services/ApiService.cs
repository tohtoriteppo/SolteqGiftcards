using System.Net.Http.Headers;

public class ApiService
{
    private readonly HttpClient _httpClient;

    public ApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> GetDataAsync(string token)
    {
        Dictionary<string, string> queryParams = new Dictionary<string, string>
        {
            ["leftUsages_min"] = "1", // has some usage left
            ["remaining_min"] = "0.001", // has some value left, assuming 1 cent is smallest value
            ["expires_min"] = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"), // is not expired
            ["status"] = "Active" // is active
        };

        using FormUrlEncodedContent content = new FormUrlEncodedContent(queryParams);
        string queryString = await content.ReadAsStringAsync();
        using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"/gift-cards/v1/Giftcard?{queryString}");

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        HttpResponseMessage response = await _httpClient.SendAsync(request);

        response.EnsureSuccessStatusCode();

        string jsonString = await response.Content.ReadAsStringAsync();

        return jsonString;
    }
}