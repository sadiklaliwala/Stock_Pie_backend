using Stock_Pie.Domain.Entities;

namespace Stock_Pie.Application.Interfaces
{
    public interface IOrderItemRepository
    {
        Task AddAsync(OrderItem item);
        Task<OrderItem?> GetByIdAsync(long id);
        Task<OrderItem?> GetByOrderIdAsync(Guid orderId);
        Task UpdateAsync(OrderItem item);
        Task RemoveAsync(OrderItem item);
        Task<int> SaveChangesAsync();
    }
}
