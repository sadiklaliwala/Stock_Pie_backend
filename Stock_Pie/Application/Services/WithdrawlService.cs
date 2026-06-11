using Stock_Pie.Application.Interfaces;
using Stock_Pie.Domain.Entities;
using Stock_Pie.Application.Dto;
using System.Security.Cryptography;
using System.Text;

namespace Stock_Pie.Application.Services
{
    public class WithdrawlService : IWithdrawlService
    {
        private readonly IWithdrawlRepository _repo;
        private readonly IWalletRepository _walletRepo;
        private readonly IUserRepository _userRepo;
        private readonly ILogger<WithdrawlService> _logger;

        public WithdrawlService(IWithdrawlRepository repo, IWalletRepository walletRepo, IUserRepository userRepo, ILogger<WithdrawlService> logger)
        {
            _repo = repo;
            _walletRepo = walletRepo;
            _userRepo = userRepo;
            _logger = logger;
        }

        private static string? ComputeSha256Hash(string? raw)
        {
            if (string.IsNullOrEmpty(raw)) return null;
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(raw);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToHexString(hash);
        }

        public async Task<WithdrawalResponseDto> RequestWithdrawal(decimal amount, Guid userId, string? bankAccountNumber)
        {
            if (amount <= 0) throw new ArgumentOutOfRangeException(nameof(amount), amount, "Amount must be greater than zero.");
            var wallet = await _walletRepo.GetByUserIdAsync(userId) ?? throw new KeyNotFoundException($"Wallet not found for user '{userId}'.");
            if (wallet.Balance < amount) throw new InvalidOperationException("Insufficient wallet balance.");

            var user = await _userRepo.GetByIdAsync(userId) ?? throw new KeyNotFoundException($"User '{userId}' not found.");

            // Enforce that user has previously provided a bank account at registration/profile
            if (string.IsNullOrEmpty(user.BankAccountHash))
            {
                _logger.LogWarning("User {UserId} attempted withdrawal without saved bank account", userId);
                throw new InvalidOperationException("No bank account on file. Please add a bank account in your profile before requesting a withdrawal.");
            }

            // Optionally, log masked last4 only
            _logger.LogInformation("Processing withdrawal for user {UserId}, amount {Amount}, accountLast4 {Last4}", userId, amount, user.BankAccountLast4 ?? "");

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
                BankAccountNumber = user.BankAccountLast4  // store masked last4 in withdrawal record instead of full string
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
            var withdrawal = await _repo.GetByIdAsync(WithdrawalId)
    ?? throw new KeyNotFoundException($"Withdrawal '{WithdrawalId}' not found.");
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
