using ShelfLife.DTOs;
using ShelfLife.Models;

namespace ShelfLife.Repository.Base
{
    public interface IDeliveryRepository
    {
        // Check if delivery person has active delivery
        Task<bool> HasActiveDeliveryAsync(int deliveryPersonId);

        // Get available orders for delivery
        Task<IEnumerable<DeliveryOrderDisplayDTO>> GetAvailableOrdersForDeliveryAsync(DeliveryOrderFilterDTO? filter = null);

        // Get active delivery for person
        Task<DeliveryDetailDTO?> GetActiveDeliveryByPersonAsync(int deliveryPersonId);

        // Get delivery history
        Task<IEnumerable<DeliveryDetailDTO>> GetDeliveryHistoryAsync(int deliveryPersonId);

        // Get delivery by ID
        Task<DeliveryDetailDTO?> GetDeliveryByIdAsync(int deliveryId);

        // Create delivery for order
        Task<DeliveryDetailDTO?> CreateDeliveryForOrderAsync(int orderId, int deliveryPersonId);

        // Update delivery status
        Task<DeliveryDetailDTO?> UpdateDeliveryStatusAsync(int deliveryId, DeliveryStatus newStatus);

        // Start delivering (changes order status to DELIVERING)
        Task<DeliveryDetailDTO?> StartDeliveringAsync(int deliveryId);

        // Confirm delivery by delivery person
        Task<DeliveryDetailDTO?> ConfirmDeliveryByPersonAsync(int deliveryId);

        // Confirm delivery by buyer
        Task<DeliveryDetailDTO?> ConfirmDeliveryByBuyerAsync(int deliveryId);

        // Cancel delivery
        Task<bool> CancelDeliveryAsync(int deliveryId);

        // Get delivery person stats
        Task<DeliveryPersonStatsDTO> GetDeliveryPersonStatsAsync(int deliveryPersonId);

        // Clean up stale deliveries (ASSIGNED for > 1 week)
        Task<int> CleanupStaleDeliveriesAsync();
    }
}