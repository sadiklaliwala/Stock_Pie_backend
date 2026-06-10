using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Stock_Pie.Application.Dto
{
    public class PortfolioDto
    {
        public Guid Id { get; set; }

        [Required]
        public string Symbol { get; set; } = null!;

        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalQuantity { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal AverageBuyPrice { get; set; }

        public DateTime LastUpdated { get; set; }
    }
}