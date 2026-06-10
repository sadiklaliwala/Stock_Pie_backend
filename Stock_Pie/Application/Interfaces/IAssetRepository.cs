using Stock_Pie.Domain.Entities;

namespace Stock_Pie.Application.Interfaces
{
    public interface IAssetRepository
    {
        Task AddAsync(Asset asset);
        Task<Asset?> GetByIdAsync(Guid assetId);
        Task<Asset?> GetByUserAndIdAsync(Guid userId, Guid assetId);
        Task<List<Asset>> GetByUserAsync(Guid userId);
        Task<Asset?> FindByUserAndCoinAsync(Guid userId, string coinId);
        Task UpdateAsync(Asset asset);
        Task RemoveAsync(Asset asset);
        Task<int> SaveChangesAsync();
    }
}
