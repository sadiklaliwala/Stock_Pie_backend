using Microsoft.EntityFrameworkCore;
using Stock_Pie.Application.Interfaces;
using Stock_Pie.Domain.Entities;
using Stock_Pie.Infrastructure.Persistence;

namespace Stock_Pie.Infrastructure.Persistence.Repositories
{
    public class WalletRepository : IWalletRepository
    {
        private readonly AppDbContext _db;

        public WalletRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(Wallet wallet)
        {
            await _db.Wallets.AddAsync(wallet);
        }

        public async Task<Wallet?> GetByIdAsync(Guid id)
        {
            return await _db.Wallets.Include(w => w.User).SingleOrDefaultAsync(w => w.Id == id);
        }

        public async Task<Wallet?> GetByUserIdAsync(Guid userId)
        {
            return await _db.Wallets.Include(w => w.User).SingleOrDefaultAsync(w => w.UserId == userId);
        }

        public async Task RemoveAsync(Wallet wallet)
        {
            _db.Wallets.Remove(wallet);
            await Task.CompletedTask;
        }

        public async Task UpdateAsync(Wallet wallet)
        {
            _db.Wallets.Update(wallet);
            await Task.CompletedTask;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _db.SaveChangesAsync();
        }
    }
}
