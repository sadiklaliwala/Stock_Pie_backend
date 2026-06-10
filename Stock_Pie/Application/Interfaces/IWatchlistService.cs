using Stock_Pie.Application.Dto;
using Stock_Pie.Domain.Entities;

namespace Stock_Pie.Application.Interfaces
{
    public interface IWatchlistService
    {
        Task<WatchListResponseDto?> FindUserWatchList(Guid UserId);
        Task<WatchList>  CreateWatchList(Guid userId);
        Task<WatchList>  FindById(Guid Id);
        Task<Coin> AddItemToWatchListAsync(Coin coin, Guid userId);
        Task<bool> RemoveCoinFromWatchlistAsync(string coinId, Guid userId);
    }
}
