using Microsoft.AspNetCore.Mvc;
using ShelfLife.DTOs;
using ShelfLife.Models;
using ShelfLife.Repository.Base;

namespace ShelfLife.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeliveryPersonController : ControllerBase
    {
        private readonly IDeliveryPersonRepository _deliveryPersonRepo;
        private readonly IDeliveryRepository _deliveryRepo;

        public DeliveryPersonController(
            IDeliveryPersonRepository deliveryPersonRepo,
            IDeliveryRepository deliveryRepo)
        {
            _deliveryPersonRepo = deliveryPersonRepo;
            _deliveryRepo = deliveryRepo;
        }

        // GET: api/DeliveryPerson/{deliveryPersonId}/profile
        // Get delivery person profile
        [HttpGet("{deliveryPersonId}/profile")]
        public async Task<ActionResult<DeliveryPersonProfileDTO>> GetProfile(int deliveryPersonId)
        {
            var profile = await GetDeliveryPersonProfile(deliveryPersonId);
            if (profile == null)
                return NotFound(new { message = "Delivery person not found" });

            return Ok(profile);
        }

        // PUT: api/DeliveryPerson/{deliveryPersonId}/profile
        // Update delivery person profile
        [HttpPut("{deliveryPersonId}/profile")]
        public async Task<ActionResult<DeliveryPersonProfileDTO>> UpdateProfile(
            int deliveryPersonId,
            [FromBody] UpdateDeliveryPersonProfileDTO updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var deliveryPerson = await _deliveryPersonRepo.GetDeliveryPersonByIdAsync(deliveryPersonId);
            if (deliveryPerson == null)
                return NotFound(new { message = "Delivery person not found" });

            deliveryPerson.Name = updateDto.Name;
            deliveryPerson.Phone = updateDto.Phone;
            deliveryPerson.Email = updateDto.Email;
            deliveryPerson.City = updateDto.City;
            deliveryPerson.VehicleType = updateDto.VehicleType;

            var updated = await _deliveryPersonRepo.UpdateDeliveryPersonAsync(deliveryPerson);
            if (updated == null)
                return StatusCode(500, new { message = "Failed to update profile" });

            var profile = await GetDeliveryPersonProfile(deliveryPersonId);
            return Ok(profile);
        }

        // PUT: api/DeliveryPerson/{deliveryPersonId}/availability
        // Toggle availability status
        [HttpPut("{deliveryPersonId}/availability")]
        public async Task<ActionResult> UpdateAvailability(
            int deliveryPersonId,
            [FromBody] UpdateAvailabilityDTO availabilityDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Check if they have an active delivery
            if (availabilityDto.IsAvailable == false)
            {
                var hasActiveDelivery = await _deliveryRepo.HasActiveDeliveryAsync(deliveryPersonId);
                if (hasActiveDelivery)
                    return BadRequest(new { message = "Cannot mark as unavailable while you have an active delivery" });
            }

            var success = await _deliveryPersonRepo.UpdateAvailabilityAsync(deliveryPersonId, availabilityDto.IsAvailable);
            if (!success)
                return NotFound(new { message = "Delivery person not found" });

            return Ok(new { message = "Availability updated successfully", isAvailable = availabilityDto.IsAvailable });
        }

        // POST: api/DeliveryPerson/{deliveryPersonId}/change-password
        // Change password
        [HttpPost("{deliveryPersonId}/change-password")]
        public async Task<ActionResult> ChangePassword(
            int deliveryPersonId,
            [FromBody] ChangePasswordDTO passwordDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var deliveryPerson = await _deliveryPersonRepo.GetDeliveryPersonByIdAsync(deliveryPersonId);
            if (deliveryPerson == null)
                return NotFound(new { message = "Delivery person not found" });

            // NOTE: In production, implement proper password hashing
            if (deliveryPerson.Password != passwordDto.CurrentPassword)
                return BadRequest(new { message = "Current password is incorrect" });

            deliveryPerson.Password = passwordDto.NewPassword; // Should be hashed
            await _deliveryPersonRepo.UpdateDeliveryPersonAsync(deliveryPerson);

            return Ok(new { message = "Password changed successfully" });
        }

        // GET: api/DeliveryPerson/{deliveryPersonId}/stats
        // Get delivery person statistics
        [HttpGet("{deliveryPersonId}/stats")]
        public async Task<ActionResult<DeliveryPersonStatsDTO>> GetStats(int deliveryPersonId)
        {
            var stats = await _deliveryRepo.GetDeliveryPersonStatsAsync(deliveryPersonId);
            return Ok(stats);
        }

        // GET: api/DeliveryPerson/{deliveryPersonId}/earnings
        // Get detailed earnings breakdown
        [HttpGet("{deliveryPersonId}/earnings")]
        public async Task<ActionResult<DeliveryPersonEarningsDTO>> GetEarnings(int deliveryPersonId)
        {
            var deliveryPerson = await _deliveryPersonRepo.GetDeliveryPersonByIdAsync(deliveryPersonId);
            if (deliveryPerson == null)
                return NotFound(new { message = "Delivery person not found" });

            var deliveries = await _deliveryRepo.GetDeliveryHistoryAsync(deliveryPersonId);
            var completedDeliveries = deliveries.Where(d => d.Status == DeliveryStatus.DELIVERED).ToList();

            var earnings = new DeliveryPersonEarningsDTO
            {
                TotalEarnings = deliveryPerson.TotalEarnings,
                TotalDeliveries = completedDeliveries.Count,
                AverageEarningsPerDelivery = completedDeliveries.Any()
                    ? deliveryPerson.TotalEarnings / completedDeliveries.Count
                    : 0,
                SaleDeliveries = completedDeliveries.Count(d => d.OrderType == OrderType.SALE),
                SwapDeliveries = completedDeliveries.Count(d => d.OrderType == OrderType.SWAP),
                ThisMonthEarnings = CalculateMonthlyEarnings(completedDeliveries)
            };

            return Ok(earnings);
        }

        // DELETE: api/DeliveryPerson/{deliveryPersonId}
        // Delete/deactivate account
        [HttpDelete("{deliveryPersonId}")]
        public async Task<ActionResult> DeleteAccount(int deliveryPersonId)
        {
            // Check if they have active deliveries
            var hasActiveDelivery = await _deliveryRepo.HasActiveDeliveryAsync(deliveryPersonId);
            if (hasActiveDelivery)
                return BadRequest(new { message = "Cannot delete account while you have active deliveries" });

            var success = await _deliveryPersonRepo.DeleteDeliveryPersonAsync(deliveryPersonId);
            if (!success)
                return NotFound(new { message = "Delivery person not found" });

            return Ok(new { message = "Account deleted successfully" });
        }

        // Helper methods

        private async Task<DeliveryPersonProfileDTO?> GetDeliveryPersonProfile(int deliveryPersonId)
        {
            var deliveryPerson = await _deliveryPersonRepo.GetDeliveryPersonByIdAsync(deliveryPersonId);
            if (deliveryPerson == null)
                return null;

            return new DeliveryPersonProfileDTO
            {
                DeliveryPersonID = deliveryPerson.DeliveryPersonID,
                Name = deliveryPerson.Name,
                Phone = deliveryPerson.Phone,
                Email = deliveryPerson.Email,
                City = deliveryPerson.City,
                VehicleType = deliveryPerson.VehicleType,
                AverageRating = deliveryPerson.AverageRating,
                TotalDeliveries = deliveryPerson.TotalDeliveries,
                IsAvailable = deliveryPerson.IsAvailable,
                TotalEarnings = deliveryPerson.TotalEarnings,
                CreatedAt = deliveryPerson.CreatedAt
            };
        }

        private static decimal CalculateMonthlyEarnings(List<DeliveryDetailDTO> deliveries)
        {
            var firstDayOfMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
            var thisMonthDeliveries = deliveries.Where(d => d.DeliveredAt >= firstDayOfMonth);

            decimal earnings = 0;
            foreach (var delivery in thisMonthDeliveries)
            {
                if (delivery.OrderType == OrderType.SALE)
                {
                    earnings += delivery.DeliveryFee * 0.8m; // 80% to delivery person
                }
                else if (delivery.OrderType == OrderType.SWAP)
                {
                    earnings += delivery.DeliveryFee * 0.5m; // 50% to delivery person
                }
            }

            return earnings;
        }
    }
}