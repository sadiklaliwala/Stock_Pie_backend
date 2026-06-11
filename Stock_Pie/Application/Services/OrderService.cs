using Stock_Pie.Application.Interfaces;
using Stock_Pie.Domain.Entities;

namespace Stock_Pie.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepo;
        private readonly IWalletService _walletService;
        private readonly ITransactionRepository _txRepo;
        private readonly IUserRepository _userRepo;
        private readonly IAssetRepository _assetRepo;

        public OrderService(
            IOrderRepository orderRepo,
            IWalletService walletService,
            ITransactionRepository txRepo,
            IUserRepository userRepo,
            IAssetRepository assetRepo)
        {
            _orderRepo = orderRepo;
            _walletService = walletService;
            _txRepo = txRepo;
            _userRepo = userRepo;
            _assetRepo = assetRepo;
        }

        public async Task<Order> CreateOrderAsync(User user, OrderItem orderItem, OrderType orderType)
        {
            var order = new Order
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                OrderType = orderType,
                Status = OrderStatus.Pending,
                OrderItem = orderItem,
                Price = (decimal)orderItem.BuyPrice,
                TimeStamp = DateTime.UtcNow
            };

            await _orderRepo.AddAsync(order);
            await _orderRepo.SaveChangesAsync();

            return order;
        }

        public async Task<Order?> GetOrderByIdAsync(Guid orderId)
        {
            return await _orderRepo.GetByIdAsync(orderId);
        }

        public async Task<List<Order>> GetAllOrdersOfUserAsync(Guid userId, OrderType orderType, string assetSymbol)
        {
            var orders = await _orderRepo.GetByUserAsync(userId);

            return orders
                .Where(o =>
                    o.OrderType == orderType &&
                    (string.IsNullOrEmpty(assetSymbol) ||
                     o.OrderItem?.Coin?.Id == assetSymbol))
                .ToList();
        }

        public async Task<Order> ProcessOrderAsync(Coin coin, double quantity, OrderType orderType, User user)
        {
            if (orderType == OrderType.Buy)
                return await BuyAssest(coin, quantity, user);

            if (orderType == OrderType.Sell)
                return await SellAssest(coin, quantity, user);

            throw new ArgumentOutOfRangeException(nameof(orderType), orderType, "Invalid order type.");
        }

        public async Task<Order> BuyAssest(Coin coin, double quantity, User user)
        {
            var item = new OrderItem
            {
                Quantity = quantity,
                Coin = coin,
                BuyPrice = (double)coin.CurrentPrice
            };

            var order = await CreateOrderAsync(user, item, OrderType.Buy);

            await _walletService.PayOrderPaymentAsync(order, user.Id);

            var tx = new Transaction
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Symbol = coin.Id ?? coin.Symbol ?? string.Empty,
                Type = TransactionType.Buy,
                Quantity = (decimal)quantity,
                PriceAtTransaction = (decimal)coin.CurrentPrice,
                TotalAmount = (decimal)quantity * coin.CurrentPrice,
                CreatedAt = DateTime.UtcNow
            };

            await _txRepo.AddAsync(tx);
            await _txRepo.SaveChangesAsync();

            var existing = await _assetRepo.FindByUserAndCoinAsync(
                user.Id,
                coin.Id ?? string.Empty);

            if (existing != null)
            {
                var totalQty = existing.Quantity + quantity;

                var totalCost =
                    (existing.BuyPrice * existing.Quantity) +
                    ((double)coin.CurrentPrice * quantity);

                existing.Quantity = totalQty;
                existing.BuyPrice = totalCost / totalQty;

                await _assetRepo.UpdateAsync(existing);
            }
            else
            {
                var asset = new Asset
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    CoinId = coin.Id,
                    Quantity = quantity,
                    BuyPrice = (double)coin.CurrentPrice
                };

                await _assetRepo.AddAsync(asset);
            }

            await _assetRepo.SaveChangesAsync();

            return order;
        }
        
        public async Task<Order> SellAssest(Coin coin, double quantity, User user)
        {
            var item = new OrderItem
            {
                Quantity = quantity,
                Coin = coin,
                BuyPrice = (double)coin.CurrentPrice
            };

            var order = await CreateOrderAsync(user, item, OrderType.Sell);

            var existing = await _assetRepo.FindByUserAndCoinAsync(
                user.Id,
                coin.Id ?? string.Empty);

            if (existing == null)
                throw new KeyNotFoundException("Asset not found.");
            if (existing.Quantity < quantity)
                throw new InvalidOperationException("Insufficient asset quantity.");

            existing.Quantity -= quantity;

            if (existing.Quantity <= 0)
                await _assetRepo.RemoveAsync(existing);
            else
                await _assetRepo.UpdateAsync(existing);

            await _assetRepo.SaveChangesAsync();

            var tx = new Transaction
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Symbol = coin.Id ?? coin.Symbol ?? string.Empty,
                Type = TransactionType.Sell,
                Quantity = (decimal)quantity,
                PriceAtTransaction = (decimal)coin.CurrentPrice,
                TotalAmount = (decimal)quantity * coin.CurrentPrice,
                CreatedAt = DateTime.UtcNow
            };

            await _txRepo.AddAsync(tx);
            await _txRepo.SaveChangesAsync();

            await _walletService.AddBalanceAsync(
                user.Id,
                (decimal)quantity * coin.CurrentPrice);

            return order;
        }
    }
}