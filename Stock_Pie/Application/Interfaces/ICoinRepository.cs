using Stock_Pie.Domain.Entities;

namespace Stock_Pie.Application.Interfaces
{
    public interface ICoinRepository
    {
        Task<Coin?> GetByIdAsync(string id);
        Task AddAsync(Coin coin);
        Task<int> SaveChangesAsync();
        Task UpdateAsync(Coin coin);


    }
}
