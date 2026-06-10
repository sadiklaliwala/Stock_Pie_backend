using Stock_Pie.Domain.Entities;

namespace Stock_Pie.Application.Interfaces
{
    public interface IWalletService
    {
        Task<Wallet?> FindByEmailAsync(string email);
        Task<Wallet?> GetUserWalletAsync(Guid userId);
        Task<Wallet> CreateWalletForUserAsync(Guid userId, decimal initialBalance = 0);
        Task<Wallet> AddBalanceAsync(Guid userId, decimal amount);
        Task<Wallet> WalletToWalletTransferAsync(Guid senderUserId, Guid receiverWalletId, decimal amount);
        Task<Wallet> PayOrderPaymentAsync(Order order, Guid userId);
    }
}
