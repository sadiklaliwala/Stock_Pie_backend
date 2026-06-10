using Stock_Pie.Application.Dto;
using Stock_Pie.Application.Interfaces;
using Stock_Pie.Domain.Entities;

namespace Stock_Pie.Application.Services
{
    public class WatchlistService : IWatchlistService
    {
        private readonly IWatchlistRepository _repo;
        private readonly ICoinService _coinService;
        private readonly ICoinRepository _coinRepo;

        public WatchlistService(IWatchlistRepository repo, ICoinService coinService, ICoinRepository coinRepo)
        {
            _repo = repo;
            _coinService = coinService;
            _coinRepo = coinRepo;
        }

        public async Task<bool> RemoveCoinFromWatchlistAsync(string coinId, Guid userId)
        {
            var wl = await _repo.GetByUserIdAsync(userId);
            if (wl == null) return false;

            var coin = wl.Coins.FirstOrDefault(c => c.Id == coinId);
            if (coin == null) return false;

            wl.Coins.Remove(coin);
            await _repo.UpdateAsync(wl);
            await _repo.SaveChangesAsync();
            return true;
        }
        public async Task<WatchListResponseDto?> FindUserWatchList(Guid userId)
        {
            var res = await _repo.GetByUserIdAsync(userId);
            if (res == null) return null;

            return new WatchListResponseDto
            {
                Id = res.Id,
                UserId = res.UserId,
                Email = res.User?.Email,
                FullName = res.User?.FullName,
                Coins = res.Coins?.Select(c => new WatchListCoinDto
                {
                    Id = c.Id,
                    Symbol = c.Symbol,
                    Name = c.Name,
                    CurrentPrice = c.CurrentPrice,
                    Image = c.Image
                }).ToList() ?? new List<WatchListCoinDto>()
            };
        }

        public async Task<WatchList> CreateWatchList(Guid userId)
        {
            // return existing if already created
            var existing = await _repo.GetByUserIdAsync(userId);
            if (existing != null) return existing;

            var wl = new WatchList
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Coins = new List<Coin>()
                // removed User = user (stub object causes EF issues)
            };

            await _repo.AddAsync(wl);
            await _repo.SaveChangesAsync();
            return wl;
        }

        public async Task<WatchList> FindById(Guid id)
        {
            return await _repo.GetByIdAsync(id)
                ?? throw new InvalidOperationException("WatchList not found");
        }

        public async Task<Coin> AddItemToWatchListAsync(Coin coin, Guid userId)
        {
            // resolve coin from DB or external API
            Coin? resolvedCoin = null;
            if (!string.IsNullOrEmpty(coin.Id))
                resolvedCoin = await _coinService.FindByIdAsync(coin.Id);

            var toAdd = resolvedCoin ?? coin;

            // get or create watchlist — no need for separate /create endpoint
            var wl = await _repo.GetByUserIdAsync(userId);
            if (wl == null)
            {
                wl = new WatchList
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Coins = new List<Coin>()
                };
                await _repo.AddAsync(wl);
            }

            // ensure coin exists in DB
            var existingCoin = await _coinRepo.GetByIdAsync(toAdd.Id ?? string.Empty);
            if (existingCoin == null)
            {
                await _coinRepo.AddAsync(toAdd);
                existingCoin = toAdd;
            }

            // add to watchlist if not already there
            if (!wl.Coins.Any(c => c.Id == existingCoin.Id))
            {
                wl.Coins.Add(existingCoin);
                await _repo.UpdateAsync(wl);
            }

            await _repo.SaveChangesAsync();
            return existingCoin;
        }
    }
}