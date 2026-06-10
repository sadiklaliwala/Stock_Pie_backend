namespace Stock_Pie.Application.Dto
{
    public class StockApiDto
    {
        public required string Name { get; set; }
        public required string Symbol { get; set; }

        public required string Exchange { get; set; }

        public string? MicCode { get; set; }
        public string? Country { get; set; }
        public required string Currency { get; set; }
        public required string Type { get; set; }
    }
}
