using System.Net.Http.Json;

namespace GLMS.Web.Services
{
    public class AuthApiService
    {
        private readonly HttpClient _http;

        public AuthApiService(IHttpClientFactory factory)
        {
            _http = factory.CreateClient();
        }

        public async Task<LoginResponse?> Login(string username, string password, string baseUrl)
        {
            var response = await _http.PostAsJsonAsync(
                $"{baseUrl}api/auth/login",
                new
                {
                    Username = username,
                    Password = password
                });

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<LoginResponse>();
        }
    }

    public class LoginResponse
    {
        public string Username { get; set; }
        public string Role { get; set; }
    }
}