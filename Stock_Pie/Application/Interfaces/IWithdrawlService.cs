using Stock_Pie.Domain.Entities;
using Stock_Pie.Application.Dto;

namespace Stock_Pie.Application.Interfaces
{
    public interface IWithdrawlService
    {
        Task<List<WithdrawalResponseDto>> GetAllWithdrawalRequest();
        Task<List<WithdrawalResponseDto>> GetUsersWithdrawalHistory(Guid UserId);
        Task<WithdrawalResponseDto> ProcedWithWidrawal(Guid WithdrawalId, bool Accept);
        Task<WithdrawalResponseDto> RequestWithdrawal(decimal amount, Guid userId, string? bankAccountNumber);

    }
}
