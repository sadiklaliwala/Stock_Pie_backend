using System;

namespace Stock_Pie.Application.Dto
{
    public class PortfolioResponseDto
    {
        public Guid Id { get; set; }
        public string Symbol { get; set; } = null!;
        public decimal TotalQuantity { get; set; }
        public decimal AverageBuyPrice { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}