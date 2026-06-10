using Microsoft.EntityFrameworkCore;
using Stock_Pie.Application.Interfaces;
using Stock_Pie.Domain.Entities;
using Stock_Pie.Infrastructure.Persistence;

namespace Stock_Pie.Infrastructure.Persistence.Repositories
{
    public class OrderItemRepository : IOrderItemRepository
    {
        private readonly AppDbContext _db;

        public OrderItemRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(OrderItem item)
        {
            await _db.Set<OrderItem>().AddAsync(item);
        }

        public async Task<OrderItem?> GetByIdAsync(long id)
        {
            return await _db.Set<OrderItem>().Include(oi => oi.Order).Include(oi => oi.Coin).SingleOrDefaultAsync(oi => oi.Id == id);
        }

        public async Task<OrderItem?> GetByOrderIdAsync(Guid orderId)
        {
            return await _db.Set<OrderItem>().Include(oi => oi.Order).Include(oi => oi.Coin).SingleOrDefaultAsync(oi => oi.OrderId == orderId);
        }

        public async Task UpdateAsync(OrderItem item)
        {
            _db.Set<OrderItem>().Update(item);
            await Task.CompletedTask;
        }

        public async Task RemoveAsync(OrderItem item)
        {
            _db.Set<OrderItem>().Remove(item);
            await Task.CompletedTask;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _db.SaveChangesAsync();
        }
    }
}
