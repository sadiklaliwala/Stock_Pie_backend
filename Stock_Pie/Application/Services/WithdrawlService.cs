using Stock_Pie.Application.Interfaces;
using Stock_Pie.Domain.Entities;
using Stock_Pie.Application.Dto;

namespace Stock_Pie.Application.Services
{
    public class WithdrawlService : IWithdrawlService
    {
        private readonly IWithdrawlRepository _repo;
        private readonly IWalletRepository _walletRepo;

        public WithdrawlService(IWithdrawlRepository repo, IWalletRepository walletRepo)
        {
            _repo = repo;
            _walletRepo = walletRepo;
        }

        public async Task<WithdrawalResponseDto> RequestWithdrawal(decimal amount, Guid userId, string? bankAccountNumber)
        {
            if (amount <= 0) throw new InvalidOperationException("Amount must be positive");
            // ensure wallet has funds
            var wallet = await _walletRepo.GetByUserIdAsync(userId) ?? throw new InvalidOperationException("Wallet not found");
            if (wallet.Balance < amount) throw new InvalidOperationException("Insufficient wallet balance");

            // deduct immediately
            wallet.Balance -= amount;
            await _walletRepo.UpdateAsync(wallet);
            await _walletRepo.SaveChangesAsync();

            var wd = new Withdrawal
            {
                Id = Guid.NewGuid(),
                Amount = amount,
                UserId = userId,
                Status = WithdrawalStatus.Pending,
                LocalDateTime = DateTime.UtcNow,
                BankAccountNumber = bankAccountNumber  // added
            };

            await _repo.AddAsync(wd);
            await _repo.SaveChangesAsync();

            return new WithdrawalResponseDto
            {
                Id = wd.Id,
                UserId = wd.UserId,
                Amount = wd.Amount,
                Status = wd.Status,
                LocalDateTime = wd.LocalDateTime
            };
        }

        public async Task<WithdrawalResponseDto> ProcedWithWidrawal(Guid WithdrawalId, Boolean Accept)
        {
            // synchronous signature per interface — do simple lookup via repository synchronously is not available
            // we'll implement a blocking call for simplicity
            var withdrawal = await _repo.GetByIdAsync(WithdrawalId) ?? throw new InvalidOperationException("Withdrawal not found");
            if (Accept)
            {
                withdrawal.Status = WithdrawalStatus.Completed;
            }
            else
            {
                withdrawal.Status = WithdrawalStatus.Failed;
                // refund user wallet
                var wallet = await _walletRepo.GetByUserIdAsync(withdrawal.UserId);
                if (wallet != null)
                {
                    wallet.Balance += withdrawal.Amount;
                    await _walletRepo.UpdateAsync(wallet);
                    await _walletRepo.SaveChangesAsync();
                }
            }

            await _repo.SaveChangesAsync();

            return new WithdrawalResponseDto
            {
                Id = withdrawal.Id,
                UserId = withdrawal.UserId,
                Amount = withdrawal.Amount,
                Status = withdrawal.Status,
                LocalDateTime = withdrawal.LocalDateTime
            };
        }

        public async Task<List<WithdrawalResponseDto>> GetUsersWithdrawalHistory(Guid UserId)
        {
            var data = await _repo.GetByUserAsync(UserId);
            return data.Select(
                w => new WithdrawalResponseDto
            {
                Id = w.Id,
                UserId = w.UserId,
                Amount = w.Amount,
                Status = w.Status,
                LocalDateTime = w.LocalDateTime,
                BankAccountNumber = w.BankAccountNumber,
                User = w.User == null ? null : new UserSummaryDto { Id = w.User.Id, Email = w.User.Email, FullName = w.User.FullName }
            }).ToList();
        }

        public async Task<List<WithdrawalResponseDto>> GetAllWithdrawalRequest()
        {
            var data = await _repo.GetAllAsync();
            return data.Select(w => new WithdrawalResponseDto
            {
                Id = w.Id,
                UserId = w.UserId,
                Amount = w.Amount,
                Status = w.Status,
                LocalDateTime = w.LocalDateTime,
                BankAccountNumber = w.BankAccountNumber,
                User = w.User == null ? null : new UserSummaryDto { Id = w.User.Id, Email = w.User.Email, FullName = w.User.FullName }
            }).ToList();
        }
    }
}
