using System.ComponentModel.DataAnnotations;

namespace Stock_Pie.Application.Dto
{
    public class AddBalanceRequest
    {
        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }
    }

    public class WalletTransferRequest
    {
        [Required]
        public Guid ReceiverWalletId { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }
    }
}
