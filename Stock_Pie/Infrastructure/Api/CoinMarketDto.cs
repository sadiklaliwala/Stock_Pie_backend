using System.Text.Json.Serialization;

namespace Stock_Pie.Infrastructure.Api
{
    public partial class CoinService
    {
        private class CoinMarketDto
        {
            public string? Id { get; set; }
            public string? Symbol { get; set; }
            public string? Name { get; set; }
            public string? Image { get; set; }
            [JsonPropertyName("current_price")] public decimal CurrentPrice { get; set; }
            [JsonPropertyName("market_cap")] public long? MarketCap { get; set; }
            [JsonPropertyName("market_cap_rank")] public int? MarketCapRank { get; set; }
            [JsonPropertyName("fully_diluted_valuation")] public long? FullyDilutedValuation { get; set; }
            [JsonPropertyName("total_volume")] public decimal? TotalVolume { get; set; }
            [JsonPropertyName("high_24h")] public decimal? High24h { get; set; }
            [JsonPropertyName("low_24h")] public decimal? Low24h { get; set; }
            [JsonPropertyName("price_change_24h")] public decimal? PriceChange24h { get; set; }
            [JsonPropertyName("price_change_percentage_24h")] public decimal? PriceChangePercentage24h { get; set; }
            [JsonPropertyName("market_cap_change_24h")] public decimal? MarketCapChange24h { get; set; }
            [JsonPropertyName("market_cap_change_percentage_24h")] public decimal? MarketCapChangePercentage24h { get; set; }
            [JsonPropertyName("circulating_supply")] public decimal? CirculatingSupply { get; set; }
            [JsonPropertyName("total_supply")] public decimal? TotalSupply { get; set; }
            [JsonPropertyName("max_supply")] public decimal? MaxSupply { get; set; }
            public decimal? Ath { get; set; }
            [JsonPropertyName("ath_change_percentage")] public decimal? AthChangePercentage { get; set; }
            [JsonPropertyName("ath_date")] public DateTime? AthDate { get; set; }
            public decimal? Atl { get; set; }
            [JsonPropertyName("atl_change_percentage")] public decimal? AtlChangePercentage { get; set; }
            [JsonPropertyName("atl_date")] public DateTime? AtlDate { get; set; }
            public RoiDto? Roi { get; set; }
            [JsonPropertyName("last_updated")] public DateTime? LastUpdated { get; set; }
        }
    }
}
