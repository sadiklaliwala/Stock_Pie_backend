using Stock_Pie.Application.Dto;
using Stock_Pie.Domain.Entities;

namespace Stock_Pie.Application.Interfaces
{
    public interface IWatchlistRepository
    {
        Task<WatchList?> GetByUserIdAsync(Guid userId);
        Task<WatchList?> GetByIdAsync(Guid id);
        Task AddAsync(WatchList watchList);
        Task UpdateAsync(WatchList watchList);
        Task<int> SaveChangesAsync();
    }
}
