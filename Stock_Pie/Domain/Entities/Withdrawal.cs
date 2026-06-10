namespace Stock_Pie.Domain.Entities
{
    //public enum WithdrawalType
    //{
    //    BankTransfer,
    //    PayPal,
    //    CryptoCurrency
    //}

    public enum WithdrawalStatus
    {
        Pending,
        Completed,
        Failed
    }

    public class Withdrawal
    {
        public Guid Id { get; set; }
        //public WithdrawalType Type { get; set; }

        public WithdrawalStatus Status { get; set; }

        public Decimal  Amount{ get; set; }

        public Guid UserId { get; set; }
        public User? User { get; set; }
        public DateTime LocalDateTime { get; set; } = DateTime.UtcNow;
        public string? BankAccountNumber { get; set; }

    }
}
