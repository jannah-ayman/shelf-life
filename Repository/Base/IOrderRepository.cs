using ShelfLife.DTOs;
using ShelfLife.Models;

namespace ShelfLife.Repository.Base
{
    public interface IOrderRepository
    {
        Task<IEnumerable<OrderDisplayDTO>> GetUserIncomingOrdersAsync(int userId);
        Task<IEnumerable<OrderDisplayDTO>> GetUserOutgoingOrdersAsync(int userId);
        Task<OrderDisplayDTO?> GetOrderByIdAsync(int orderId);
        Task<Order?> CreateOrderAsync(Order order);
        Task<Order?> UpdateOrderAsync(Order order);
        Task<bool> UserOwnsOrderListingAsync(int userId, int orderId);
        Task<bool> UserIsOrderBuyerAsync(int userId, int orderId);
    }
}
