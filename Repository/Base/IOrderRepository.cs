using ShelfLife.DTOs;
using ShelfLife.Models;

namespace ShelfLife.Repository.Base
{
    public interface IOrderRepository
    {
        // Order retrieval methods
        Task<IEnumerable<OrderDisplayDTO>> GetUserIncomingOrdersAsync(int userId);
        Task<IEnumerable<OrderDisplayDTO>> GetUserOutgoingOrdersAsync(int userId);
        Task<OrderDisplayDTO?> GetOrderByIdAsync(int orderId);

        // Payment breakdown
        Task<PaymentBreakdownDTO?> GetPaymentBreakdownAsync(int orderId);

        // Basic CRUD operations
        Task<Order?> CreateOrderAsync(Order order);
        Task<Order?> UpdateOrderAsync(Order order);
        Task<bool> DeleteOrderAsync(int id);

        // Order ownership checks
        Task<bool> UserOwnsOrderListingAsync(int userId, int orderId);
        Task<bool> UserIsOrderBuyerAsync(int userId, int orderId);

        // Helper methods for order creation
        Task<BookListing?> GetListingByIdAsync(int listingId);
        Task<User?> GetUserByIdAsync(int userId);
        Task<bool> CanUserSwapAsync(int userId);
        Task<bool> IsListingAvailableForOrderAsync(int listingId, int quantity);

        // Specific order type creation methods
        Task<Order?> CreateSaleOrderAsync(int buyerId, CreateSaleOrderDTO dto);
        Task<Order?> CreateSwapOrderAsync(int buyerId, CreateSwapOrderDTO dto);

        // Swap response method
        Task<bool> RespondToSwapAsync(int orderId, bool accept);
    }
}