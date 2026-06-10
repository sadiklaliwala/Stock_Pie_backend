using Stock_Pie.Domain.Entities;
using System.Runtime.InteropServices;

namespace Stock_Pie.Application.Interfaces
{
    public interface IOrderService
    {
        Task<Order> CreateOrderAsync(User user, OrderItem orderItem, OrderType orderType);

        Task<Order?> GetOrderByIdAsync(Guid orderId);

        Task<List<Order>> GetAllOrdersOfUserAsync(Guid userId, OrderType orderType, string assetSymbol);

        Task<Order> ProcessOrderAsync(Coin coin, double quantity, OrderType orderType, User user);

        Task<Order> BuyAssest(Coin coin, double quantity, User user);

        Task<Order> SellAssest(Coin coin, double quantity, User user);
    }
}
