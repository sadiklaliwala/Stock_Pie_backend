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
            var user = await _userRepo.GetByIdAsync(userId);
            if (user == null) throw new InvalidOperationException("User not found");
            var existing = await _repo.GetByUserIdAsync(userId);
            if (existing != null) throw new InvalidOperationException("Wallet already exists for user");

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
            if (amount <= 0) throw new InvalidOperationException("Amount must be positive");
            var wallet = await _repo.GetByUserIdAsync(userId) ?? throw new InvalidOperationException("Wallet not found");
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
            if (amount <= 0) throw new InvalidOperationException("Amount must be positive");
            var senderWallet = await _repo.GetByUserIdAsync(senderUserId) ?? throw new InvalidOperationException("Sender wallet not found");
            var receiverWallet = await _repo.GetByIdAsync(receiverWalletId) ?? throw new InvalidOperationException("Receiver wallet not found");
            if (senderWallet.Balance < amount) throw new InvalidOperationException("Insufficient balance");

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
            var wallet = await _repo.GetByUserIdAsync(userId) ?? throw new InvalidOperationException("Wallet not found");
            if (wallet.Balance < order.Price) throw new InvalidOperationException("Insufficient funds to pay order");

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
