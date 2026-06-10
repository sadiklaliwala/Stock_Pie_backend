using Microsoft.EntityFrameworkCore;
using Stock_Pie.Application.Dto;
using Stock_Pie.Application.Interfaces;
using Stock_Pie.Domain.Entities;
using Stock_Pie.Infrastructure.Persistence;
using System;

namespace Stock_Pie.Infrastructure.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _txRepo;
        private readonly IPortfolioRepository _portfolioRepo;

        public TransactionService(ITransactionRepository txRepo, IPortfolioRepository portfolioRepo)
        {
            _txRepo = txRepo;
            _portfolioRepo = portfolioRepo;
        }

        public async Task<Transaction> CreateAsync(Guid userId, TransactionCreateDto dto)
        {
            
            var total = dto.PriceAtTransaction * dto.Quantity;

            var tx = new Transaction
            {
                UserId = userId,
                Symbol = dto.Symbol,
                Type = dto.Type,
                Quantity = dto.Quantity,
                PriceAtTransaction = dto.PriceAtTransaction,
                TotalAmount = total,
                CreatedAt = DateTime.UtcNow
            };

            await _txRepo.AddAsync(tx);

            // Update portfolio only for buys, sells reduce quantity
            if (dto.Type == TransactionType.Buy)
            {
                var portfolio = await _portfolioRepo.GetByUserAndSymbolAsync(userId, dto.Symbol);
                if (portfolio == null)
                {
                    portfolio = new Portfolio
                    {
                        UserId = userId,
                        Symbol = dto.Symbol,
                        TotalQuantity = dto.Quantity,
                        AverageBuyPrice = dto.PriceAtTransaction,
                        LastUpdated = DateTime.UtcNow
                    };
                    await _portfolioRepo.AddAsync(portfolio);
                }
                else
                {
                    var totalCost = portfolio.AverageBuyPrice * portfolio.TotalQuantity;
                    totalCost += dto.PriceAtTransaction * dto.Quantity;
                    portfolio.TotalQuantity += dto.Quantity;
                    portfolio.AverageBuyPrice = portfolio.TotalQuantity == 0 ? 0 : totalCost / portfolio.TotalQuantity;
                    portfolio.LastUpdated = DateTime.UtcNow;
                }
            }
            else
            {
                var portfolio = await _portfolioRepo.GetByUserAndSymbolAsync(userId, dto.Symbol);
                if (portfolio != null)
                {
                    portfolio.TotalQuantity -= dto.Quantity;
                    if (portfolio.TotalQuantity < 0) portfolio.TotalQuantity = 0;
                    portfolio.LastUpdated = DateTime.UtcNow;
                    if (portfolio.TotalQuantity == 0)
                    {
                        await _portfolioRepo.RemoveAsync(portfolio);
                    }
                }
            }

            await _txRepo.SaveChangesAsync();
            return tx;
        }

        public async Task<IEnumerable<Transaction>> GetByUserAsync(Guid userId)
        {
            return await _txRepo.GetByUserAsync(userId);
        }
    }
}