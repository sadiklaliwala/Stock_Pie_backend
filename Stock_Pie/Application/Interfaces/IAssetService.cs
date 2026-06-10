using Stock_Pie.Domain.Entities;

namespace Stock_Pie.Application.Interfaces
{
    public interface IAssetService
    {
        Task<Asset> CreateAssetAsync(User user, Coin coin, double quantity);

        Task<Asset?> GetAssetByIdAsync(Guid assetId);

        Task<Asset?> GetAssetByUserIdAndIdAsync(Guid userId, Guid assetId);

        Task<List<Asset>> GetUsersAssetsAsync(Guid userId);

        Task<Asset> UpdateAssetAsync(Guid assetId, double quantity, double buyPrice);

        Task<Asset?> FindAssetByUserIdAndCoinIdAsync(Guid userId, string coinId);

        Task DeleteAssetAsync(Guid assetId);
    }
}
