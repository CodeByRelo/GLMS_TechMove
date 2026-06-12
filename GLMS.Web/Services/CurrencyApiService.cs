using GLMS.Web.Services.Interfaces;

namespace GLMS.Web.Services
{
    public class CurrencyApiService : ICurrencyService
    {
        private readonly HttpClient _httpClient;

        public CurrencyApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<decimal> ConvertUsdToZar(decimal usdAmount)
        {
            // Calls your API endpoint (you will create this in GLMS.API)
            var response = await _httpClient.GetAsync($"api/currency/convert?usdAmount={usdAmount}");

            if (!response.IsSuccessStatusCode)
                throw new Exception("Currency conversion API failed.");

            var result = await response.Content.ReadFromJsonAsync<decimal>();

            return result;
        }
    }
}