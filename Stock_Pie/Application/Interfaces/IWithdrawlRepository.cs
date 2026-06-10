using Stock_Pie.Domain.Entities;

namespace Stock_Pie.Application.Interfaces
{
    public interface IWithdrawlRepository
    {
        Task AddAsync(Withdrawal withdrawal);
        Task<Withdrawal?> GetByIdAsync(Guid id);
        Task<List<Withdrawal>> GetByUserAsync(Guid userId);
        Task<List<Withdrawal>> GetAllAsync();
        Task RemoveAsync(Withdrawal withdrawal);
        Task<int> SaveChangesAsync();
    }
}
