using Microsoft.EntityFrameworkCore;
using Stock_Pie.Application.Interfaces;
using Stock_Pie.Domain.Entities;
using Stock_Pie.Infrastructure.Persistence;

namespace Stock_Pie.Infrastructure.Persistence.Repositories
{
    public class AssetRepository : IAssetRepository
    {
        private readonly AppDbContext _db;

        public AssetRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(Asset asset)
        {
            await _db.Assets.AddAsync(asset);
        }

        public async Task<Asset?> GetByIdAsync(Guid assetId)
        {
            return await _db.Assets.Include(a => a.Coin).Include(a => a.User).SingleOrDefaultAsync(a => a.Id == assetId);
        }

        public async Task<Asset?> GetByUserAndIdAsync(Guid userId, Guid assetId)
        {
            return await _db.Assets.Include(a => a.Coin).Include(a => a.User).Where(a => a.UserId == userId && a.Id == assetId).SingleOrDefaultAsync();
        }

        public async Task<List<Asset>> GetByUserAsync(Guid userId)
        {
            return await _db.Assets.Include(a => a.Coin).Include(a => a.User).Where(a => a.UserId == userId).ToListAsync();
        }

        public async Task<Asset?> FindByUserAndCoinAsync(Guid userId, string coinId)
        {
            return await _db.Assets.Include(a => a.Coin).Include(a => a.User).Where(a => a.UserId == userId && a.CoinId == coinId).SingleOrDefaultAsync();
        }

        public async Task UpdateAsync(Asset asset)
        {
            _db.Assets.Update(asset);
            await Task.CompletedTask;
        }

        public async Task RemoveAsync(Asset asset)
        {
            _db.Assets.Remove(asset);
            await Task.CompletedTask;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _db.SaveChangesAsync();
        }
    }
}
