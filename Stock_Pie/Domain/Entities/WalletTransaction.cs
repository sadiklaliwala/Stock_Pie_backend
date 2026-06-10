namespace Stock_Pie.Domain.Entities
{
    public enum WalletTransactionType
    {
        Deposit,
        Withdrawal,
        Transfer
    }
    public class WalletTransaction
    {
        public Guid  Id { get; set; }
        public DateTime   Date{ get; set; }
        public string?  Purpose { get; set; }
        public Guid  TransferId { get; set; }
        public WalletTransactionType Type { get; set; }
        public decimal Amount { get; set; }
        public Guid WalletId { get; set; }

        // Navigation property (Many Transactions -> One Wallet)
        public Wallet? Wallet { get; set; }
    }
}
