using Microsoft.EntityFrameworkCore;
using Stock_Pie.Application.Dto;
using Stock_Pie.Application.Interfaces;
using Stock_Pie.Domain.Entities;
using Stock_Pie.Infrastructure.Persistence;

namespace Stock_Pie.Application.Services
{
    public interface ITradingService
    {
        Task<Transaction> BuyAsync(Guid userId, TransactionCreateDto dto);
        Task<Transaction> SellAsync(Guid userId, TransactionCreateDto dto);
    }

    public class TradingService : ITradingService
    {
        private readonly AppDbContext _db;
        private readonly IPortfolioService _portfolioService;

        public TradingService(AppDbContext db, IPortfolioService portfolioService)
        {
            _db = db;
            _portfolioService = portfolioService;
        }

        public async Task<Transaction> BuyAsync(Guid userId, TransactionCreateDto dto)
        {
            if (dto.Quantity <= 0) throw new InvalidOperationException("Quantity must be greater than zero");
            if (dto.PriceAtTransaction < 0) throw new InvalidOperationException("Price must be non-negative");

            using var tx = await _db.Database.BeginTransactionAsync();
            try
            {
                var transaction = new Transaction
                {
                    UserId = userId,
                    Symbol = dto.Symbol,
                    Type = TransactionType.Buy,
                    Quantity = dto.Quantity,
                    PriceAtTransaction = dto.PriceAtTransaction,
                    TotalAmount = dto.PriceAtTransaction * dto.Quantity,
                    CreatedAt = DateTime.UtcNow
                };
                _db.Transactions.Add(transaction);

                await _portfolioService.UpsertPortfolioForBuyAsync(userId, dto.Symbol, dto.Quantity, dto.PriceAtTransaction);

                await _db.SaveChangesAsync();
                await tx.CommitAsync();
                return transaction;
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        public async Task<Transaction> SellAsync(Guid userId, TransactionCreateDto dto)
        {
            if (dto.Quantity <= 0) throw new InvalidOperationException("Quantity must be greater than zero");
            if (dto.PriceAtTransaction < 0) throw new InvalidOperationException("Price must be non-negative");

            using var tx = await _db.Database.BeginTransactionAsync();
            try
            {
                var portfolio = await _portfolioService.GetByUserAndSymbolAsync(userId, dto.Symbol) ?? throw new InvalidOperationException("No holdings for symbol");
                if (portfolio.TotalQuantity < dto.Quantity) throw new InvalidOperationException("Insufficient quantity to sell");

                var transaction = new Transaction
                {
                    UserId = userId,
                    Symbol = dto.Symbol,
                    Type = TransactionType.Sell,
                    Quantity = dto.Quantity,
                    PriceAtTransaction = dto.PriceAtTransaction,
                    TotalAmount = dto.PriceAtTransaction * dto.Quantity,
                    CreatedAt = DateTime.UtcNow
                };
                _db.Transactions.Add(transaction);

                await _portfolioService.ApplySellAsync(userId, dto.Symbol, dto.Quantity);

                await _db.SaveChangesAsync();
                await tx.CommitAsync();
                return transaction;
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }
    }
}
