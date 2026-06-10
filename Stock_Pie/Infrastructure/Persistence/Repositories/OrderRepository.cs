using Microsoft.EntityFrameworkCore;
using Stock_Pie.Application.Interfaces;
using Stock_Pie.Domain.Entities;
using Stock_Pie.Infrastructure.Persistence;

namespace Stock_Pie.Infrastructure.Persistence.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _db;

        public OrderRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(Order order)
        {
            await _db.Orders.AddAsync(order);
        }

        public async Task<Order?> GetByIdAsync(Guid orderId)
        {
            return await _db.Orders.Include(o => o.OrderItem).Include(o => o.User).SingleOrDefaultAsync(o => o.Id == orderId);
        }

        public async Task<List<Order>> GetByUserAsync(Guid userId)
        {
            return await _db.Orders.Include(o => o.OrderItem).Where(o => o.UserId == userId).ToListAsync();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _db.SaveChangesAsync();
        }
    }
}
