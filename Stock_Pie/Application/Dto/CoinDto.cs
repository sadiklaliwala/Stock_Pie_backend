using System;

namespace Stock_Pie.Application.Dto
{
    public class CoinSummaryDto
    {
        public string? Id { get; set; }
        public string? Symbol { get; set; }
        public string? Name { get; set; }
        public string? Image { get; set; }
        public decimal CurrentPrice { get; set; }
        public long MarketCap { get; set; }
        public int MarketCapRank { get; set; }
        public decimal PriceChangePercentage24h { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
