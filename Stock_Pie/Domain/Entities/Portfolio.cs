using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Stock_Pie.Domain.Entities
{
    public class Portfolio
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid UserId { get; set; }

        [Required]
        public string Symbol { get; set; } = null!;

        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalQuantity { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal AverageBuyPrice { get; set; }

        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

        // Navigation
        public User? User { get; set; }
    }
}