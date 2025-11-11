using Microsoft.EntityFrameworkCore;
using ShelfLife.Models;
using ShelfLife.Repository.Base;

namespace ShelfLife.Repository
{
    public class DeliveryPersonRepository : IDeliveryPersonRepository
    {
        private readonly DBcontext _context;

        public DeliveryPersonRepository(DBcontext context)
        {
            _context = context;
        }

        public async Task<DeliveryPerson?> GetDeliveryPersonByIdAsync(int deliveryPersonId)
        {
            return await _context.DeliveryPeople
                .Include(dp => dp.Deliveries)
                .FirstOrDefaultAsync(dp => dp.DeliveryPersonID == deliveryPersonId);
        }

        public async Task<IEnumerable<DeliveryPerson>> GetAllDeliveryPeopleAsync()
        {
            return await _context.DeliveryPeople
                .OrderByDescending(dp => dp.AverageRating)
                .ToListAsync();
        }

        public async Task<IEnumerable<DeliveryPerson>> GetAvailableDeliveryPeopleAsync(string? city = null)
        {
            var query = _context.DeliveryPeople
                .Where(dp => dp.IsAvailable);

            if (!string.IsNullOrWhiteSpace(city))
            {
                query = query.Where(dp => dp.City == city);
            }

            return await query
                .OrderByDescending(dp => dp.AverageRating)
                .ToListAsync();
        }

        public async Task<DeliveryPerson?> CreateDeliveryPersonAsync(DeliveryPerson deliveryPerson)
        {
            _context.DeliveryPeople.Add(deliveryPerson);
            await _context.SaveChangesAsync();
            return deliveryPerson;
        }

        public async Task<DeliveryPerson?> UpdateDeliveryPersonAsync(DeliveryPerson deliveryPerson)
        {
            _context.DeliveryPeople.Update(deliveryPerson);
            await _context.SaveChangesAsync();
            return deliveryPerson;
        }

        public async Task<bool> DeleteDeliveryPersonAsync(int deliveryPersonId)
        {
            var deliveryPerson = await _context.DeliveryPeople.FindAsync(deliveryPersonId);
            if (deliveryPerson == null)
                return false;

            _context.DeliveryPeople.Remove(deliveryPerson);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateAvailabilityAsync(int deliveryPersonId, bool isAvailable)
        {
            var deliveryPerson = await _context.DeliveryPeople.FindAsync(deliveryPersonId);
            if (deliveryPerson == null)
                return false;

            deliveryPerson.IsAvailable = isAvailable;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}