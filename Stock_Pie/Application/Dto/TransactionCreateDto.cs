using Stock_Pie.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace Stock_Pie.Application.Dto
{
    public class TransactionCreateDto
    {
        [Required]
        public string Symbol { get; set; } = null!;

        [Required]
        public TransactionType Type { get; set; }  // "Buy" or "Sell"

        [Range(0.00000001, double.MaxValue)]
        public decimal Quantity { get; set; }

        [Range(0.0, double.MaxValue)]
        public decimal PriceAtTransaction { get; set; }
    }
}