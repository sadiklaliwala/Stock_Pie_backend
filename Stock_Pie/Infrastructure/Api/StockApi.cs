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

            _apiKey = configuration["StockSettings:ApiKey"] ?? "";
            _baseUrl = configuration["StockSettings:BaseUrl"] ?? "";

            if (string.IsNullOrEmpty(_apiKey) || string.IsNullOrEmpty(_baseUrl))
            {
                throw new ArgumentNullException(nameof(configuration));
            }
        }

        public async Task<List<StockApiDto>?> GetStockDataAsync()
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/stocks");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var result = JsonSerializer.Deserialize<StockApiResponse>(json, options);

                return result == null ? throw new Exception("Failed to deserialize stock data.") : result.Data;

                //if (result?.Data != null)
                //{
                //    result.Data.ForEach(stock =>
                //    {
                //        Console.WriteLine("name " + stock.Name);
                //    });
                //}
            }
            else
            {
                throw new Exception("Failed to fetch stock data. Status code: " + response.StatusCode);
            }
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

        //public async Task<Dictionary<string, decimal>> GetPricesAsync(List<string> symbols)
        //{
        //    var symbolString = string.Join(",", symbols);

        //    // ✅ Use QUOTE endpoint (important)
        //    var url = $"{_baseUrl}/quote?symbol={symbolString}&apikey={_apiKey}";

        //    var response = await _httpClient.GetAsync(url);

        //    if (!response.IsSuccessStatusCode)
        //        throw new Exception("Failed to fetch prices");

        //    var json = await response.Content.ReadAsStringAsync();

        //    // 🔥 STEP 1: HANDLE ERROR RESPONSE
        //    if (json.Contains("\"code\""))
        //    {
        //        var error = JsonSerializer.Deserialize<ApiErrorResponse>(json);
        //        throw new Exception($"TwelveData Error: {error?.Message}");
        //    }

        //    var options = new JsonSerializerOptions
        //    {
        //        PropertyNameCaseInsensitive = true
        //    };

        //    // 🔥 STEP 2: DESERIALIZE QUOTE RESPONSE
        //    var data = JsonSerializer.Deserialize<Dictionary<string, QuoteResponse>>(json, options);

        //    if (data == null)
        //        return new Dictionary<string, decimal>();

        //    // 🔥 STEP 3: SAFE PARSING
        //    return data
        //        .Where(x => x.Value != null && !string.IsNullOrEmpty(x.Value.Close))
        //        .ToDictionary(
        //            x => x.Key,
        //            x =>
        //            {
        //                decimal.TryParse(x.Value.Close, out var price);
        //                return price;
        //            }
        //        );
        //}

        //public async Task GetStockDataAsync()
        //{
        //    var response = await httpClient.GetAsync("https://api.twelvedata.com/stocks");

        //    if (response.IsSuccessStatusCode)
        //    {
        //        var json = await response.Content.ReadAsStringAsync();

        //        var stocks = JsonSerializer.Deserialize<List<StockApiDto>>(json);

        //        stocks.ForEach(stock =>
        //        {
        //            Console.WriteLine("name" + stock.Name);
        //        });


        //    }
    }
}

