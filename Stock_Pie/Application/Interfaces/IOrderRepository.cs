using Stock_Pie.Domain.Entities;

namespace Stock_Pie.Application.Interfaces
{
    public interface IOrderRepository
    {
        Task AddAsync(Order order);
        Task<Order?> GetByIdAsync(Guid orderId);
        Task<List<Order>> GetByUserAsync(Guid userId);
        Task<int> SaveChangesAsync();
    }
}
