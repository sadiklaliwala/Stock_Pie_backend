using System.ComponentModel.DataAnnotations;
using Stock_Pie.Domain.Entities;

namespace Stock_Pie.Application.Dto
{
    public class CreateOrderRequest
    {
        [Required]
        public OrderType OrderType { get; set; }

        [Required]
        public string Symbol { get; set; } = null!;

        [Range(0.00000001, double.MaxValue)]
        public double Quantity { get; set; }

        
    }
}
