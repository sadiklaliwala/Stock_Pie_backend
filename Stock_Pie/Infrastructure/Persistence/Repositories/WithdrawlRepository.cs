using Microsoft.EntityFrameworkCore;
using Stock_Pie.Application.Interfaces;
using Stock_Pie.Domain.Entities;
using Stock_Pie.Infrastructure.Persistence;

namespace Stock_Pie.Infrastructure.Persistence.Repositories
{
    public class WithdrawlRepository : IWithdrawlRepository
    {
        private readonly AppDbContext _db;

        public WithdrawlRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(Withdrawal withdrawal)
        {
            await _db.Withdrawals.AddAsync(withdrawal);
        }

        public async Task<Withdrawal?> GetByIdAsync(Guid id)
        {
            return await _db.Withdrawals.Include(w => w.User).SingleOrDefaultAsync(w => w.Id == id);
        }

        public async Task<List<Withdrawal>> GetByUserAsync(Guid userId)
        {
            return await _db.Withdrawals.Where(w => w.UserId == userId).ToListAsync();
        }

        public async Task<List<Withdrawal>> GetAllAsync()
        {
            return await _db.Withdrawals.Include(w => w.User).ToListAsync();
        }

        public async Task RemoveAsync(Withdrawal withdrawal)
        {
            _db.Withdrawals.Remove(withdrawal);
            await Task.CompletedTask;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _db.SaveChangesAsync();
        }
    }
}
