namespace Stock_Pie.Domain.Entities
{
    public class WatchList
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public User? User { get; set; }
        public List<Coin> Coins { get; set; } = new List<Coin>();
    }
}
