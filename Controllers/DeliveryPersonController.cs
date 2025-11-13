using Microsoft.AspNetCore.Mvc;
using ShelfLife.DTOs;
using ShelfLife.Models;
using ShelfLife.Repository.Base;
using System.Security.Cryptography;
using System.Text;

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

        // POST: api/DeliveryPerson/register
        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] DeliveryPersonRegisterDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Check if email already exists
            var existingPerson = await _deliveryPersonRepo.GetDeliveryPersonByEmailAsync(dto.Email);
            if (existingPerson != null)
                return Conflict(new { message = "Email already registered" });

            // Hash password
            var hashedPassword = HashPassword(dto.Password);

            var deliveryPerson = new DeliveryPerson
            {
                Name = dto.Name,
                Phone = dto.Phone,
                Email = dto.Email,
                Password = hashedPassword,
                City = dto.City,
                VehicleType = dto.VehicleType,
                AverageRating = 0,
                TotalDeliveries = 0,
                IsAvailable = true,
                TotalEarnings = 0,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _deliveryPersonRepo.CreateDeliveryPersonAsync(deliveryPerson);
            if (created == null)
                return StatusCode(500, new { message = "Failed to register delivery person" });

            return Ok(new
            {
                message = "Registration successful",
                deliveryPersonId = created.DeliveryPersonID
            });
        }

        // POST: api/DeliveryPerson/login
        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] DeliveryPersonLoginDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var deliveryPerson = await _deliveryPersonRepo.GetDeliveryPersonByEmailAsync(dto.Email);
            if (deliveryPerson == null)
                return Unauthorized(new { message = "Invalid credentials" });

            var hashedPassword = HashPassword(dto.Password);
            if (deliveryPerson.Password != hashedPassword)
                return Unauthorized(new { message = "Invalid credentials" });

            return Ok(new
            {
                message = "Login successful",
                deliveryPerson = new
                {
                    deliveryPerson.DeliveryPersonID,
                    deliveryPerson.Name,
                    deliveryPerson.Email,
                    deliveryPerson.Phone,
                    deliveryPerson.City,
                    deliveryPerson.VehicleType,
                    deliveryPerson.IsAvailable
                }
            });
        }

        // GET: api/DeliveryPerson/{deliveryPersonId}/profile
        [HttpGet("{deliveryPersonId}/profile")]
        public async Task<ActionResult<DeliveryPersonProfileDTO>> GetProfile(int deliveryPersonId)
        {
            var profile = await GetDeliveryPersonProfile(deliveryPersonId);
            if (profile == null)
                return NotFound(new { message = "Delivery person not found" });

            return Ok(profile);
        }

        // PUT: api/DeliveryPerson/{deliveryPersonId}/profile
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
        [HttpPut("{deliveryPersonId}/availability")]
        public async Task<ActionResult> UpdateAvailability(
            int deliveryPersonId,
            [FromBody] UpdateAvailabilityDTO availabilityDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

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

            // Hash and verify current password
            var currentHash = HashPassword(passwordDto.CurrentPassword);
            if (deliveryPerson.Password != currentHash)
                return BadRequest(new { message = "Current password is incorrect" });

            // Hash new password
            deliveryPerson.Password = HashPassword(passwordDto.NewPassword);
            await _deliveryPersonRepo.UpdateDeliveryPersonAsync(deliveryPerson);

            return Ok(new { message = "Password changed successfully" });
        }

        // GET: api/DeliveryPerson/{deliveryPersonId}/stats
        [HttpGet("{deliveryPersonId}/stats")]
        public async Task<ActionResult<DeliveryPersonStatsDTO>> GetStats(int deliveryPersonId)
        {
            var stats = await _deliveryRepo.GetDeliveryPersonStatsAsync(deliveryPersonId);
            return Ok(stats);
        }

        // GET: api/DeliveryPerson/{deliveryPersonId}/earnings
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
        [HttpDelete("{deliveryPersonId}")]
        public async Task<ActionResult> DeleteAccount(int deliveryPersonId)
        {
            var hasActiveDelivery = await _deliveryRepo.HasActiveDeliveryAsync(deliveryPersonId);
            if (hasActiveDelivery)
                return BadRequest(new { message = "Cannot delete account while you have active deliveries" });

            var success = await _deliveryPersonRepo.DeleteDeliveryPersonAsync(deliveryPersonId);
            if (!success)
                return NotFound(new { message = "Delivery person not found" });

            return Ok(new { message = "Account deleted successfully" });
        }

        // Helper methods
        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

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
                // 80% to delivery person for both SALE and SWAP
                earnings += delivery.DeliveryFee * 0.8m;
            }

            return earnings;
        }
    }
}