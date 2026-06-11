using Microsoft.EntityFrameworkCore;
using Stock_Pie.Application.Dto;
using Stock_Pie.Application.Interfaces;
using Stock_Pie.Domain.Entities;
using Stock_Pie.Infrastructure.Persistence;

namespace Stock_Pie.Infrastructure.Services
{
    public class PortfolioService : IPortfolioService
    {
        private readonly IPortfolioRepository _portfolioRepo;

        public PortfolioService(IPortfolioRepository portfolioRepo)
        {
            _portfolioRepo = portfolioRepo;
        }

        public async Task<Portfolio> UpsertPortfolioForBuyAsync(Guid userId, string symbol, decimal buyQuantity, decimal price)
        {
            if (buyQuantity > 0)
            {

            var portfolio = await _portfolioRepo.GetByUserAndSymbolAsync(userId, symbol);
            if (portfolio == null)
            {
                portfolio = new Portfolio
                {
                    UserId = userId,
                    Symbol = symbol,
                    TotalQuantity = buyQuantity,
                    AverageBuyPrice = price,
                    LastUpdated = DateTime.UtcNow
                };
                await _portfolioRepo.AddAsync(portfolio);
            }
            else
            {
                var totalCost = portfolio.AverageBuyPrice * portfolio.TotalQuantity;
                totalCost += price * buyQuantity;
                portfolio.TotalQuantity += buyQuantity;
                portfolio.AverageBuyPrice = portfolio.TotalQuantity == 0 ? 0 : totalCost / portfolio.TotalQuantity;
                portfolio.LastUpdated = DateTime.UtcNow;
            }

            return portfolio;
            }
            throw new ArgumentOutOfRangeException(nameof(buyQuantity), buyQuantity, "Buy quantity must be greater than zero.");

        }

        public async Task<Portfolio> ApplySellAsync(Guid userId, string symbol, decimal sellQuantity)
        {
            var portfolio = await _portfolioRepo.GetByUserAndSymbolAsync(userId, symbol);
            if (portfolio == null) throw new KeyNotFoundException($"No holdings found for symbol '{symbol}'.");
            if (sellQuantity <= 0) throw new ArgumentOutOfRangeException(nameof(sellQuantity), sellQuantity, "Sell quantity must be greater than zero.");
            if (portfolio.TotalQuantity < sellQuantity) throw new InvalidOperationException("Insufficient quantity to sell.");
            portfolio.TotalQuantity -= sellQuantity;
            if (portfolio.TotalQuantity == 0)
            {
                await _portfolioRepo.RemoveAsync(portfolio);
            }
            else
            {
                portfolio.LastUpdated = DateTime.UtcNow;
            }

            return portfolio;
        }

        public async Task<Portfolio?> GetByUserAndSymbolAsync(Guid userId, string symbol)
        {
            return await _portfolioRepo.GetByUserAndSymbolAsync(userId, symbol);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            // repository API does not have GetById; use GetByUserAndSymbol or expand repository if needed
            throw new NotImplementedException();
        }
    }
}