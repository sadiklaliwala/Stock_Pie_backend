using Stock_Pie.Domain.Entities;

namespace Stock_Pie.Application.Interfaces
{
    public interface IPortfolioService
    {
        // Update portfolio in-memory (does not call SaveChanges) — caller should persist changes in a transaction
        Task<Portfolio> UpsertPortfolioForBuyAsync(Guid userId, string symbol, decimal buyQuantity, decimal price);
        // Apply sell; throw if not enough holdings. Caller persists changes.
        Task<Portfolio> ApplySellAsync(Guid userId, string symbol, decimal sellQuantity);
        Task<Portfolio?> GetByUserAndSymbolAsync(Guid userId, string symbol);
        Task<bool> DeleteAsync(Guid id);
    }
}