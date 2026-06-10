using Microsoft.EntityFrameworkCore;
using Stock_Pie.Application.Interfaces;
using Stock_Pie.Domain.Entities;
using Stock_Pie.Infrastructure.Persistence;

namespace Stock_Pie.Infrastructure.Persistence.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly AppDbContext _db;

        public PaymentRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(PaymentOrder order)
        {
            await _db.Set<PaymentOrder>().AddAsync(order);
        }

        public async Task<PaymentOrder?> GetByIdAsync(Guid id)
        {
            return await _db.Set<PaymentOrder>().Include(p => p.User).SingleOrDefaultAsync(p => p.Id == id);
        }

        public async Task UpdateAsync(PaymentOrder order)
        {
            _db.Set<PaymentOrder>().Update(order);
            await Task.CompletedTask;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _db.SaveChangesAsync();
        }
    }
}
