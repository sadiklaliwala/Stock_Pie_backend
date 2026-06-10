using Stock_Pie.Domain.Entities;

namespace Stock_Pie.Application.Dto
{
    public class AssetDto
    {
        public Guid Id { get; set; }
        public CoinDto? Coin { get; set; }
        public UserSummaryDto? User { get; set; }
        public double Quantity { get; set; }
        public double BuyPrice { get; set; }
    }

    public class CoinDto
    {
        public string? Id { get; set; }
        public string? Symbol { get; set; }
        public string? Name { get; set; }
        public string? Image { get; set; }
        public decimal CurrentPrice { get; set; }
        public long MarketCap { get; set; }
        public int MarketCapRank { get; set; }
        public decimal TotalVolume { get; set; }
        public decimal High24h { get; set; }
        public decimal Low24h { get; set; }
        public decimal PriceChange24h { get; set; }
        public decimal PriceChangePercentage24h { get; set; }
        public DateTime LastUpdated { get; set; }
    }

    public class UserSummaryDto
    {
        public Guid Id { get; set; }
        public string? Email { get; set; }
        public string? FullName { get; set; }
    }

    public class CreateAssetDto
    {
        public string CoinId { get; set; } = null!;
        public double Quantity { get; set; }
    }

    public class UpdateAssetDto
    {
        public double Quantity { get; set; }
        public double BuyPrice { get; set; }
    }
}
