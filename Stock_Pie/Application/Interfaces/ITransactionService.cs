using Stock_Pie.Application.Dto;
using Stock_Pie.Domain.Entities;

namespace Stock_Pie.Application.Interfaces
{
    public interface ITransactionService
    {
        // Creates transaction and persists
        Task<Transaction> CreateAsync(Guid userId, TransactionCreateDto dto);
        Task<IEnumerable<Transaction>> GetByUserAsync(Guid userId);
    }
}