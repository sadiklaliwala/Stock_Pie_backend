using System;
using Stock_Pie.Domain.Entities;

namespace Stock_Pie.Application.Dto
{
    public class TransactionDto
    {
        public Guid Id { get; set; }
        public string Symbol { get; set; } = null!;
        public TransactionType Type { get; set; }
        public decimal Quantity { get; set; }
        public decimal PriceAtTransaction { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}