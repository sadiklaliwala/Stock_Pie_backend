using Stock_Pie.Domain.Entities;

namespace Stock_Pie.Application.Interfaces
{
    public interface IWalletTransactionRepository
    {
        Task AddAsync(WalletTransaction tx);
        Task<IEnumerable<WalletTransaction>> GetByWalletIdAsync(Guid walletId);
        Task<int> SaveChangesAsync();
    }
}
