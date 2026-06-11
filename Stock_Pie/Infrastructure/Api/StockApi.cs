using Stock_Pie.Application.Dto;
using System.Text.Json;

namespace Stock_Pie.Infrastructure.Api
{
    public class StockApi
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _baseUrl;

        public StockApi(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["StockSettings:ApiKey"] ?? throw new InvalidOperationException("StockSettings:ApiKey is not configured.");
            _baseUrl = configuration["StockSettings:BaseUrl"] ?? throw new InvalidOperationException("StockSettings:BaseUrl is not configured.");
        }

        public async Task<List<StockApiDto>?> GetStockDataAsync()
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/stocks");

            if (!response.IsSuccessStatusCode)
            {
                var err = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Failed to fetch stock data: {err}", null, response.StatusCode);
            }

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<StockApiResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return result?.Data ?? throw new InvalidOperationException("Failed to deserialize stock data.");
        }

        private static readonly Random _random = new();
        public decimal GetNextPrice(decimal currentPrice)
        {
            if (currentPrice <= 0)
                currentPrice = 100; // fallback default

            // Change between -3% and +3%
            var changePercent = (decimal)(_random.NextDouble() * 0.06 - 0.03);

            var newPrice = currentPrice + (currentPrice * changePercent);

            // Safety: avoid zero or negative price
            var responsePrice= Math.Round(Math.Max(newPrice,1), 2);

            return responsePrice;
        }

        
    }
}

