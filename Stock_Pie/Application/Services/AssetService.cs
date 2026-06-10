using Stock_Pie.Application.Interfaces;
using Stock_Pie.Domain.Entities;

namespace Stock_Pie.Application.Services
{
    public class AssetService : IAssetService
    {
        private readonly IAssetRepository _repo;

        public AssetService(IAssetRepository repo)
        {
            _repo = repo;
        }

        public async Task<Asset> CreateAssetAsync(User user, Coin coin, double quantity)
        {
            var asset = new Asset
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                CoinId = coin.Id,
                Quantity = quantity,
                BuyPrice = (double)coin.CurrentPrice
            };

            await _repo.AddAsync(asset);
            await _repo.SaveChangesAsync();
            return asset;
        }

        public async Task<Asset?> GetAssetByIdAsync(Guid assetId)
        {
            return await _repo.GetByIdAsync(assetId);
        }

        public async Task<Asset?> GetAssetByUserIdAndIdAsync(Guid userId, Guid assetId)
        {
            return await _repo.GetByUserAndIdAsync(userId, assetId);
        }

        public async Task<List<Asset>> GetUsersAssetsAsync(Guid userId)
        {
            return await _repo.GetByUserAsync(userId);
        }

        public async Task<Asset> UpdateAssetAsync(Guid assetId, double quantity, double buyPrice)
        {
            var asset = await _repo.GetByIdAsync(assetId) ?? throw new InvalidOperationException("Asset not found");
            asset.Quantity = quantity;
            asset.BuyPrice = buyPrice;
            await _repo.UpdateAsync(asset);
            await _repo.SaveChangesAsync();
            return asset;
        }

        public async Task<Asset?> FindAssetByUserIdAndCoinIdAsync(Guid userId, string coinId)
        {
            return await _repo.FindByUserAndCoinAsync(userId, coinId);
        }

        public async Task DeleteAssetAsync(Guid assetId)
        {
            var asset = await _repo.GetByIdAsync(assetId) ?? throw new InvalidOperationException("Asset not found");
            await _repo.RemoveAsync(asset);
            await _repo.SaveChangesAsync();
        }
    }
}
