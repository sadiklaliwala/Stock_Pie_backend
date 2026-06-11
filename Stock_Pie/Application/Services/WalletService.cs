using Stock_Pie.Application.Interfaces;
using Stock_Pie.Domain.Entities;

namespace Stock_Pie.Application.Services
{
    public class WalletService(IWalletRepository repo, IUserRepository userRepo, IWalletTransactionRepository walletTxRepo) : IWalletService
    {
        private readonly IWalletRepository _repo = repo;
        private readonly IUserRepository _userRepo = userRepo;
        private readonly IWalletTransactionRepository _walletTxRepo = walletTxRepo;

        public async Task<Wallet?> GetUserWalletAsync(Guid userId)
        {
            return await _repo.GetByUserIdAsync(userId);
        }

        public async Task<Wallet?> FindByEmailAsync(string email)
        {
            var user = await _userRepo.GetByEmailAsync(email);
            if (user == null) return null;
            return await _repo.GetByUserIdAsync(user.Id);
        }

        public async Task<Wallet> CreateWalletForUserAsync(Guid userId, decimal initialBalance = 0)
        {
            var user = await _userRepo.GetByIdAsync(userId)
    ?? throw new KeyNotFoundException($"User '{userId}' not found.");

            var existing = await _repo.GetByUserIdAsync(userId);
            if (existing != null) throw new InvalidOperationException("Wallet already exists for this user.");
            var wallet = new Wallet { UserId = userId, Balance = initialBalance, CreatedAt = DateTime.UtcNow };
            await _repo.AddAsync(wallet);
            await _repo.SaveChangesAsync();

            // add wallet transaction
            var tx = new WalletTransaction
            {
                Id = Guid.NewGuid(),
                Date = DateTime.UtcNow,
                Purpose = "Initial deposit",
                TransferId = Guid.Empty,
                Type = WalletTransactionType.Deposit,
                Amount = initialBalance,
                WalletId = wallet.Id
            };
            await _walletTxRepo.AddAsync(tx);
            await _walletTxRepo.SaveChangesAsync();

            return wallet;
        }

        public async Task<Wallet> AddBalanceAsync(Guid userId, decimal amount)
        {
            if (amount <= 0) throw new ArgumentOutOfRangeException(nameof(amount), amount, "Amount must be greater than zero.");
            var wallet = await _repo.GetByUserIdAsync(userId) ?? throw new KeyNotFoundException($"Wallet not found for user '{userId}'.");
            wallet.Balance += amount;
            await _repo.UpdateAsync(wallet);
            await _repo.SaveChangesAsync();

            var tx = new WalletTransaction
            {
                Id = Guid.NewGuid(),
                Date = DateTime.UtcNow,
                Purpose = "Top-up",
                TransferId = Guid.Empty,
                Type = WalletTransactionType.Deposit,
                Amount = amount,
                WalletId = wallet.Id
            };
            await _walletTxRepo.AddAsync(tx);
            await _walletTxRepo.SaveChangesAsync();

            return wallet;
        }

        public async Task<Wallet> WalletToWalletTransferAsync(Guid senderUserId, Guid receiverWalletId, decimal amount)
        {
            if (amount <= 0) throw new ArgumentOutOfRangeException(nameof(amount), amount, "Amount must be greater than zero.");
            var senderWallet = await _repo.GetByUserIdAsync(senderUserId) ?? throw new KeyNotFoundException($"Wallet not found for user '{senderUserId}'.");
            var receiverWallet = await _repo.GetByIdAsync(receiverWalletId) ?? throw new KeyNotFoundException($"Wallet '{receiverWalletId}' not found.");
            if (senderWallet.Balance < amount) throw new InvalidOperationException("Insufficient balance.");
            var transferId = Guid.NewGuid();

            senderWallet.Balance -= amount;
            receiverWallet.Balance += amount;

            await _repo.UpdateAsync(senderWallet);
            await _repo.UpdateAsync(receiverWallet);
            await _repo.SaveChangesAsync();

            var txOut = new WalletTransaction
            {
                Id = Guid.NewGuid(),
                Date = DateTime.UtcNow,
                Purpose = "Transfer out",
                TransferId = transferId,
                Type = WalletTransactionType.Transfer,
                Amount = -amount,
                WalletId = senderWallet.Id
            };
            var txIn = new WalletTransaction
            {
                Id = Guid.NewGuid(),
                Date = DateTime.UtcNow,
                Purpose = "Transfer in",
                TransferId = transferId,
                Type = WalletTransactionType.Transfer,
                Amount = amount,
                WalletId = receiverWallet.Id
            };

            await _walletTxRepo.AddAsync(txOut);
            await _walletTxRepo.AddAsync(txIn);
            await _walletTxRepo.SaveChangesAsync();

            return senderWallet;
        }

        public async Task<Wallet> PayOrderPaymentAsync(Order order, Guid userId)
        {
            if (order == null) throw new ArgumentNullException(nameof(order));
            var wallet = await _repo.GetByUserIdAsync(userId) ?? throw new KeyNotFoundException($"Wallet not found for user '{userId}'.");
            if (wallet.Balance < order.Price) throw new InvalidOperationException("Insufficient funds.");
            wallet.Balance -= order.Price;
            await _repo.UpdateAsync(wallet);
            await _repo.SaveChangesAsync();

            var tx = new WalletTransaction
            {
                Id = Guid.NewGuid(),
                Date = DateTime.UtcNow,
                Purpose = $"Order payment #{order.Id}",
                TransferId = Guid.Empty,
                Type = WalletTransactionType.Withdrawal,
                Amount = -order.Price,
                WalletId = wallet.Id
            };
            await _walletTxRepo.AddAsync(tx);
            await _walletTxRepo.SaveChangesAsync();
            return wallet;
        }
    }
}
