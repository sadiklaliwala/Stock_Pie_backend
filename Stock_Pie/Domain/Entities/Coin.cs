using Stock_Pie.Infrastructure.Persistence.Repositories;

namespace Stock_Pie.Domain.Entities
{

    public class Roi
    {
        public int Id { get; set; }
        public decimal Times { get; set; }
        public string? Currency { get; set; }
        public decimal Percentage { get; set; }
    }
    public class Coin
    {
        public string? Id { get; set; }
        public string? Symbol { get; set; }
        public string? Name { get; set; }
        public string? Image { get; set; }
        public decimal CurrentPrice { get; set; }
        public long MarketCap { get; set; }
        public int MarketCapRank { get; set; }
        public long? FullyDilutedValuation { get; set; }
        public decimal TotalVolume { get; set; }
        public decimal High24h { get; set; }
        public decimal Low24h { get; set; }
        public decimal PriceChange24h { get; set; }
        public decimal PriceChangePercentage24h { get; set; }
        public decimal? MarketCapChange24h { get; set; }
        public decimal MarketCapChangePercentage24h { get; set; }
        public decimal CirculatingSupply { get; set; }
        public decimal? TotalSupply { get; set; }
        public decimal? MaxSupply { get; set; }
        public decimal Ath { get; set; }
        public decimal AthChangePercentage { get; set; }
        public DateTime? AthDate { get; set; }
        public decimal Atl { get; set; }
        public decimal AtlChangePercentage { get; set; }
        public DateTime? AtlDate { get; set; }
        public Roi? Roi { get; set; }  // null in your JSON, adjust if structure is known
        public DateTime LastUpdated { get; set; }

        // One coin → many assets
        public ICollection<Asset> Assets { get; set; } = [];

        public List<WatchList> WatchLists { get; set; } = new();
    }
}
