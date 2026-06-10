using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Stock_Pie.Domain.Entities
{
    public class OrderItem
    {
        public long Id { get; set; }   // Primary Key

        public double Quantity { get; set; }

        public Coin? Coin { get; set; }

        public double BuyPrice { get; set; }

        public double SellPrice { get; set; }

        // foreign key to Order (match Order.Id type)
        public Guid OrderId { get; set; }
        public Order? Order { get; set; }
    }
}
