using Stock_Pie.Domain.Entities;

namespace Stock_Pie.Application.Interfaces
{
    public interface ICoinService
    {
        Task<List<Coin>> GetCoinListAsync(int page);

        Task<string> GetMarketChartAsync(string coinId, int days);

        Task<string> GetCoinDetailsAsync(string coinId);

        Task<Coin?> FindByIdAsync(string coinId);

        Task<string> SearchCoinAsync(string keyword);

        Task<string> GetTop50CoinsByMarketCapRankAsync();

        Task<string> GetTrendingCoinsAsync();
    }
}
