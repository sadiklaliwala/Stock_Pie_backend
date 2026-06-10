namespace Stock_Pie.Infrastructure.Api
{
    public partial class CoinService
    {
        private class RoiDto
        {
            public decimal? Times { get; set; }
            public string? Currency { get; set; }
            public decimal? Percentage { get; set; }
        }
    }
}
