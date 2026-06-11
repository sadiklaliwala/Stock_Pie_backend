using Microsoft.EntityFrameworkCore;
using Stock_Pie.Application.Interfaces;
using Stock_Pie.Domain.Entities;
using Stock_Pie.Infrastructure.Persistence;

namespace Stock_Pie.Infrastructure.Persistence.Repositories
{
    public class CoinRepository : ICoinRepository
    {
        private readonly AppDbContext _db;

        public CoinRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<Coin?> GetByIdAsync(string id)
        {
            return await _db.Coins.SingleOrDefaultAsync(c => c.Id == id);
        }

        public async Task AddAsync(Coin coin)
        {
            await _db.Coins.AddAsync(coin);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(Coin coin)
        {
            _db.Coins.Update(coin);
            await _db.SaveChangesAsync();
        }
    }
}
