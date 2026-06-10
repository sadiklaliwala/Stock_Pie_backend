using Stock_Pie.Domain.Entities;

namespace Stock_Pie.Application.Dto
{
    public class WithdrawalResponseDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public decimal Amount { get; set; }
        public WithdrawalStatus Status { get; set; }
        public DateTime LocalDateTime { get; set; }

        // Optional: include a small user summary if you want to show user info in admin views
        public UserSummaryDto? User { get; set; }
        public string? BankAccountNumber { get; set; }
    }

    public class RequestWithdrawalDto
    {
        public decimal Amount { get; set; }
    }
}
