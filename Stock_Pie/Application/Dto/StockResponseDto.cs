namespace Stock_Pie.Application.Dto
{
    public class StockListResponseDto
    {
        public required string Symbol { get; set; }
        public required string Name { get; set; }
        public required string Exchange { get; set; }
        public required string Country { get; set; }
        public required string Currency { get; set; }
        public required string Type { get; set; }

        public decimal CurrentPrice { get; set; }
    }
}
