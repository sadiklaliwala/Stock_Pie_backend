using Stock_Pie.Domain.Entities;

namespace Stock_Pie.Application.Interfaces
{
    public interface IPaymentRepository
    {
        Task AddAsync(PaymentOrder order);
        Task<PaymentOrder?> GetByIdAsync(Guid id);
        Task UpdateAsync(PaymentOrder order);
        Task<int> SaveChangesAsync();
    }
}
