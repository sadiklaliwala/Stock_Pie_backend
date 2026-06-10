namespace Stock_Pie.Domain.Entities
{
    public class Asset
    {
        public Guid Id { get; set; }
        public double Quantity { get; set; }
        public double BuyPrice { get; set; }

        // Foreign keys
        public string? CoinId { get; set; }
        public Guid UserId { get; set; }

        // Navigation properties (many-to-one)
        public Coin? Coin { get; set; }

        public User? User { get; set; }
    }
}
