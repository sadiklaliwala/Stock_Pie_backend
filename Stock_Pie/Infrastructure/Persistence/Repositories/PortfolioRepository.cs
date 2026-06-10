using Microsoft.EntityFrameworkCore;
using Stock_Pie.Application.Interfaces;
using Stock_Pie.Domain.Entities;
using Stock_Pie.Infrastructure.Persistence;

namespace Stock_Pie.Infrastructure.Persistence.Repositories
{
    public class PortfolioRepository : IPortfolioRepository
    {
        private readonly AppDbContext _db;

        public PortfolioRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<Portfolio?> GetByUserAndSymbolAsync(Guid userId, string symbol)
        {
            return await _db.Portfolios.SingleOrDefaultAsync(p => p.UserId == userId && p.Symbol == symbol);
        }

        public async Task AddAsync(Portfolio portfolio)
        {
            await _db.Portfolios.AddAsync(portfolio);
        }

        public async Task RemoveAsync(Portfolio portfolio)
        {
            _db.Portfolios.Remove(portfolio);
            await Task.CompletedTask;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _db.SaveChangesAsync();
        }
    }
}
