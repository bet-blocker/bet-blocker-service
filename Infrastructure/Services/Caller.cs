using System.Text;
using System.Text.Json;
using Infrastructure.Services.Interfaces;

namespace Infrastructure.Services
{
    public class Caller : ICaller
    {
        private readonly HttpClient _httpClient;

        public Caller(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> Call(string endpoint, HttpMethod method, object? body = null)
        {
            var request = new HttpRequestMessage(method, endpoint);

            if (body != null)
            {
                request.Content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
            }

            var response = await _httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }
    }
}