using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Caching.Memory;
using Stock_Pie.Application.Interfaces;
using Stock_Pie.Domain.Entities;

namespace Stock_Pie.Infrastructure.Api
{
    public partial class CoinService : ICoinService
    {
        private readonly HttpClient _http;
        private readonly IConfiguration _config;
        private readonly ICoinRepository _coinRepo;
        private readonly IMemoryCache _cache;

        // cache durations
        private static readonly TimeSpan ListCacheDuration = TimeSpan.FromMinutes(2);
        private static readonly TimeSpan DetailsCacheDuration = TimeSpan.FromMinutes(5);
        private static readonly TimeSpan ChartCacheDuration = TimeSpan.FromMinutes(3);
        private static readonly TimeSpan Top50CacheDuration = TimeSpan.FromMinutes(5);
        private static readonly TimeSpan TrendingCacheDuration = TimeSpan.FromMinutes(5);
        private static readonly TimeSpan SearchCacheDuration = TimeSpan.FromMinutes(2);

        public CoinService(HttpClient http, IConfiguration config, ICoinRepository coinRepo, IMemoryCache cache)
        {
            _http = http;
            _config = config;
            _coinRepo = coinRepo;
            _cache = cache;
        }

        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        public async Task<List<Coin>> GetCoinListAsync(int page)
        {
            var cacheKey = $"coin_list_{page}";
            if (_cache.TryGetValue(cacheKey, out List<Coin>? cached) && cached != null)
                return cached;

            var perPage = 10;
            var url = $"coins/markets?vs_currency=usd&order=market_cap_desc&per_page={perPage}&page={page}&sparkline=false&price_change_percentage=24h";
            var resp = await _http.GetAsync(url);

            if (!resp.IsSuccessStatusCode)
            {
                var msg = await resp.Content.ReadAsStringAsync();
                throw new Exception($"CoinGecko Error: {resp.StatusCode} - {msg}");
            }

            var stream = await resp.Content.ReadAsStreamAsync();
            var markets = await JsonSerializer.DeserializeAsync<List<CoinMarketDto>>(stream, _jsonOptions);
            var result = markets?.Select(MapMarketDto).ToList() ?? new();

            _cache.Set(cacheKey, result, ListCacheDuration);
            return result;
        }

        public async Task<string> GetMarketChartAsync(string coinId, int days)
        {
            var cacheKey = $"coin_chart_{coinId}_{days}";
            if (_cache.TryGetValue(cacheKey, out string? cached) && cached != null)
                return cached;

            var url = $"coins/{coinId}/market_chart?vs_currency=usd&days={days}";
            var resp = await _http.GetAsync(url);
            resp.EnsureSuccessStatusCode();
            var json = await resp.Content.ReadAsStringAsync();

            _cache.Set(cacheKey, json, ChartCacheDuration);
            return json;
        }

        public async Task<string> GetCoinDetailsAsync(string coinId)
        {
            var cacheKey = $"coin_details_{coinId}";
            if (_cache.TryGetValue(cacheKey, out string? cached) && cached != null)
                return cached;

            try
            {
                var url = $"coins/{coinId}?localization=false&tickers=false&market_data=true&community_data=false&developer_data=false&sparkline=false";
                var resp = await _http.GetAsync(url);
                resp.EnsureSuccessStatusCode();
                var json = await resp.Content.ReadAsStringAsync();

                var doc = JsonSerializer.Deserialize<JsonElement>(json, _jsonOptions);
                var coin = new Coin
                {
                    Id = doc.GetProperty("id").GetString(),
                    Symbol = doc.GetProperty("symbol").GetString(),
                    Name = doc.GetProperty("name").GetString(),
                    Image = doc.GetProperty("image").GetProperty("large").GetString(),
                    CurrentPrice = GetDecimalOrNull(doc, "market_data", "current_price", "usd") ?? 0,
                    MarketCap = GetLongOrNull(doc, "market_data", "market_cap", "usd") ?? 0,
                    MarketCapRank = doc.TryGetProperty("market_cap_rank", out var mcr) && mcr.ValueKind != JsonValueKind.Null
                        ? mcr.GetInt32() : 0,
                    FullyDilutedValuation = GetLongOrNull(doc, "market_data", "fully_diluted_valuation", "usd"),
                    TotalVolume = GetLongOrNull(doc, "market_data", "total_volume", "usd") ?? 0,
                    High24h = GetDecimalOrNull(doc, "market_data", "high_24h", "usd") ?? 0,
                    Low24h = GetDecimalOrNull(doc, "market_data", "low_24h", "usd") ?? 0,
                    PriceChange24h = GetDecimalOrNull(doc, "market_data", "price_change_24h") ?? 0,
                    PriceChangePercentage24h = GetDecimalOrNull(doc, "market_data", "price_change_percentage_24h") ?? 0,
                    MarketCapChange24h = GetLongOrNull(doc, "market_data", "market_cap_change_24h") ?? 0,
                    MarketCapChangePercentage24h = GetDecimalOrNull(doc, "market_data", "market_cap_change_percentage_24h") ?? 0,
                    CirculatingSupply = GetDecimalOrNull(doc, "market_data", "circulating_supply") ?? 0,
                    TotalSupply = GetDecimalOrNull(doc, "market_data", "total_supply"),
                    MaxSupply = GetDecimalOrNull(doc, "market_data", "max_supply"),
                    Ath = GetDecimalOrNull(doc, "market_data", "ath", "usd") ?? 0,
                    AthChangePercentage = GetDecimalOrNull(doc, "market_data", "ath_change_percentage", "usd") ?? 0,
                    AthDate = GetDateTimeOrNull(doc, "market_data", "ath_date", "usd"),
                    Atl = GetDecimalOrNull(doc, "market_data", "atl", "usd") ?? 0,
                    AtlChangePercentage = GetDecimalOrNull(doc, "market_data", "atl_change_percentage", "usd") ?? 0,
                    AtlDate = GetDateTimeOrNull(doc, "market_data", "atl_date", "usd"),
                    Roi = null,
                    LastUpdated = doc.TryGetProperty("last_updated", out var lu) && lu.ValueKind != JsonValueKind.Null
                        ? lu.GetDateTime() : DateTime.UtcNow
                };

                var existing = await _coinRepo.GetByIdAsync(coinId);
                if (existing == null)
                {
                    await _coinRepo.AddAsync(coin);
                    await _coinRepo.SaveChangesAsync();
                }

                _cache.Set(cacheKey, json, DetailsCacheDuration);
                return json;
            }
            catch (Exception ex)
            {
                Console.WriteLine("error " + ex);
                throw;
            }
        }

        public async Task<Coin?> FindByIdAsync(string coinId)
        {
            var coin = await _coinRepo.GetByIdAsync(coinId);
            if (coin != null) return coin;

            var details = await GetCoinDetailsAsync(coinId);
            if (string.IsNullOrEmpty(details)) throw new Exception("No coin found");

            var saved = await _coinRepo.GetByIdAsync(coinId);
            return saved ?? throw new Exception("No coin found");
        }

        public async Task<string> GetTop50CoinsByMarketCapRankAsync()
        {
            var cacheKey = "coin_top50";
            if (_cache.TryGetValue(cacheKey, out string? cached) && cached != null)
                return cached;

            var url = "coins/markets?vs_currency=usd&order=market_cap_desc&per_page=50&page=1&sparkline=false";
            var resp = await _http.GetAsync(url);
            resp.EnsureSuccessStatusCode();
            var json = await resp.Content.ReadAsStringAsync();

            _cache.Set(cacheKey, json, Top50CacheDuration);
            return json;
        }

        public async Task<string> GetTrendingCoinsAsync()
        {
            var cacheKey = "coin_trending";
            if (_cache.TryGetValue(cacheKey, out string? cached) && cached != null)
                return cached;

            var url = "search/trending";
            var resp = await _http.GetAsync(url);
            resp.EnsureSuccessStatusCode();
            var json = await resp.Content.ReadAsStringAsync();

            _cache.Set(cacheKey, json, TrendingCacheDuration);
            return json;
        }

        public async Task<string> SearchCoinAsync(string keyword)
        {
            var cacheKey = $"coin_search_{keyword.ToLower()}";
            if (_cache.TryGetValue(cacheKey, out string? cached) && cached != null)
                return cached;

            var url = $"search?query={Uri.EscapeDataString(keyword)}";
            var resp = await _http.GetAsync(url);
            resp.EnsureSuccessStatusCode();
            var json = await resp.Content.ReadAsStringAsync();

            _cache.Set(cacheKey, json, SearchCacheDuration);
            return json;
        }

        private static Coin MapMarketDto(CoinMarketDto dto)
        {
            return new Coin
            {
                Id = dto.Id,
                Symbol = dto.Symbol,
                Name = dto.Name,
                Image = dto.Image,
                CurrentPrice = dto.CurrentPrice,
                MarketCap = dto.MarketCap ?? 0,
                MarketCapRank = dto.MarketCapRank ?? 0,
                FullyDilutedValuation = dto.FullyDilutedValuation ?? 0,
                TotalVolume = dto.TotalVolume ?? 0,
                High24h = dto.High24h ?? 0,
                Low24h = dto.Low24h ?? 0,
                PriceChange24h = dto.PriceChange24h ?? 0,
                PriceChangePercentage24h = dto.PriceChangePercentage24h ?? 0,
                MarketCapChange24h = dto.MarketCapChange24h ?? 0,
                MarketCapChangePercentage24h = dto.MarketCapChangePercentage24h ?? 0,
                CirculatingSupply = dto.CirculatingSupply ?? 0,
                TotalSupply = dto.TotalSupply ?? 0,
                MaxSupply = dto.MaxSupply,
                Ath = dto.Ath ?? 0,
                AthChangePercentage = dto.AthChangePercentage ?? 0,
                AthDate = dto.AthDate ?? DateTime.MinValue,
                Atl = dto.Atl ?? 0,
                AtlChangePercentage = dto.AtlChangePercentage ?? 0,
                AtlDate = dto.AtlDate ?? DateTime.MinValue,
                Roi = dto.Roi == null ? null : new Roi { Id = 0, Times = dto.Roi.Times ?? 0, Currency = dto.Roi.Currency, Percentage = dto.Roi.Percentage ?? 0 },
                LastUpdated = dto.LastUpdated ?? DateTime.MinValue
            };
        }

        private static decimal? GetDecimalOrNull(JsonElement el, params string[] path)
        {
            var current = el;
            foreach (var key in path)
            {
                if (!current.TryGetProperty(key, out current)) return null;
            }
            if (current.ValueKind == JsonValueKind.Null) return null;
            if (current.TryGetDecimal(out var val)) return val;
            return null;
        }

        private static long? GetLongOrNull(JsonElement el, params string[] path)
        {
            var current = el;
            foreach (var key in path)
            {
                if (!current.TryGetProperty(key, out current)) return null;
            }
            if (current.ValueKind == JsonValueKind.Null) return null;
            if (current.TryGetInt64(out var longVal)) return longVal;
            if (current.TryGetDecimal(out var decVal)) return (long)decVal;
            return null;
        }

        private static DateTime? GetDateTimeOrNull(JsonElement el, params string[] path)
        {
            var current = el;
            foreach (var key in path)
            {
                if (!current.TryGetProperty(key, out current)) return null;
            }
            return current.ValueKind == JsonValueKind.Null ? null : current.GetDateTime();
        }
    }
}