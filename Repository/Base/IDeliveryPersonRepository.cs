using ShelfLife.Models;

namespace ShelfLife.Repository.Base
{
    public interface IDeliveryPersonRepository
    {
        Task<DeliveryPerson?> GetDeliveryPersonByIdAsync(int deliveryPersonId);
        Task<IEnumerable<DeliveryPerson>> GetAllDeliveryPeopleAsync();
        Task<IEnumerable<DeliveryPerson>> GetAvailableDeliveryPeopleAsync(string? city = null);
        Task<DeliveryPerson?> CreateDeliveryPersonAsync(DeliveryPerson deliveryPerson);
        Task<DeliveryPerson?> UpdateDeliveryPersonAsync(DeliveryPerson deliveryPerson);
        Task<bool> DeleteDeliveryPersonAsync(int deliveryPersonId);
        Task<bool> UpdateAvailabilityAsync(int deliveryPersonId, bool isAvailable);
        Task<decimal> UpdateAverageRatingAsync(int deliveryPersonId);
    }
}