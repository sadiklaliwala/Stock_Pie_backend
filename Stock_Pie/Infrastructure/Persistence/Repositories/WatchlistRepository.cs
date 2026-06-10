using Microsoft.EntityFrameworkCore;
using Stock_Pie.Application.Interfaces;
using Stock_Pie.Domain.Entities;
using Stock_Pie.Infrastructure.Persistence;

namespace Stock_Pie.Infrastructure.Persistence.Repositories
{
    public class WatchlistRepository : IWatchlistRepository
    {
        private readonly AppDbContext _db;

        public WatchlistRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<WatchList?> GetByUserIdAsync(Guid userId)
        {
            return await _db.WatchLists.Include(w => w.Coins).Include(e=> e.User).SingleOrDefaultAsync(w => w.UserId == userId);
        }

        public async Task<WatchList?> GetByIdAsync(Guid id)
        {
            return await _db.WatchLists.Include(w => w.Coins).SingleOrDefaultAsync(w => w.Id == id);
        }

        public async Task AddAsync(WatchList watchList)
        {
            await _db.WatchLists.AddAsync(watchList);
        }

        public async Task UpdateAsync(WatchList watchList)
        {
            _db.WatchLists.Update(watchList);
            await _db.SaveChangesAsync();
            return;
            
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _db.SaveChangesAsync();
        }
    }
}
