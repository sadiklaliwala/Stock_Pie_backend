using Microsoft.EntityFrameworkCore;
using Stock_Pie.Application.Interfaces;
using Stock_Pie.Domain.Entities;
using Stock_Pie.Infrastructure.Persistence;

namespace Stock_Pie.Infrastructure.Persistence.Repositories
{
    public class WalletTransactionRepository : IWalletTransactionRepository
    {
        private readonly AppDbContext _db;

        public WalletTransactionRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(WalletTransaction tx)
        {
            await _db.WalletTransactions.AddAsync(tx);
        }

        public async Task<IEnumerable<WalletTransaction>> GetByWalletIdAsync(Guid walletId)
        {
            return await _db.WalletTransactions.Where(wt => wt.WalletId == walletId).ToListAsync();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _db.SaveChangesAsync();
        }
    }
}
