using Stock_Pie.Domain.Entities;

namespace Stock_Pie.Application.Interfaces
{
    public interface IPortfolioRepository
    {
        Task<Portfolio?> GetByUserAndSymbolAsync(Guid userId, string symbol);
        Task AddAsync(Portfolio portfolio);
        Task RemoveAsync(Portfolio portfolio);
        Task<int> SaveChangesAsync();
    }
}
