using Microsoft.AspNetCore.Mvc;
using ShelfLife.DTOs;
using ShelfLife.Models;
using ShelfLife.Repository.Base;

namespace ShelfLife.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeliveryController : ControllerBase
    {
        private readonly IDeliveryPersonRepository _deliveryPersonRepo;
        private readonly IDeliveryRepository _deliveryRepo;
        private readonly IOrderRepository _orderRepo;

        public DeliveryController(IDeliveryRepository deliveryRepo, IOrderRepository orderRepo,
        IDeliveryPersonRepository deliveryPersonRepo)
        {
            _deliveryRepo = deliveryRepo;
            _orderRepo = orderRepo;
            _deliveryPersonRepo = deliveryPersonRepo;
        }

        // GET: api/Delivery/{deliveryPersonId}/available-orders
        [HttpGet("{deliveryPersonId}/available-orders")]
        public async Task<ActionResult<IEnumerable<DeliveryOrderDisplayDTO>>> GetAvailableOrders(
            int deliveryPersonId,
            [FromQuery] string? city = null,
            [FromQuery] decimal? maxDistance = null,
            [FromQuery] decimal? minFee = null,
            [FromQuery] decimal? maxFee = null,
            [FromQuery] OrderType? orderType = null)
        {
            var dp = await _deliveryPersonRepo.GetDeliveryPersonByIdAsync(deliveryPersonId);
            if (dp == null)
                return NotFound(new { message = "Delivery person not found" });

            // Validate fee range
            if (minFee.HasValue && maxFee.HasValue && minFee.Value > maxFee.Value)
            {
                return BadRequest(new { message = "Minimum fee cannot be greater than maximum fee" });
            }

            // Check if delivery person has an active delivery
            var hasActiveDelivery = await _deliveryRepo.HasActiveDeliveryAsync(deliveryPersonId);
            if (hasActiveDelivery)
            {
                return Ok(new List<DeliveryOrderDisplayDTO>()); // Return empty list
            }

            var filter = new DeliveryOrderFilterDTO
            {
                City = city,
                MaxDistance = maxDistance,
                MinFee = minFee,
                MaxFee = maxFee,
                OrderType = orderType
            };

            var orders = await _deliveryRepo.GetAvailableOrdersForDeliveryAsync(filter);
            return Ok(orders);
        }

        // GET: api/Delivery/{deliveryPersonId}/active-delivery
        [HttpGet("{deliveryPersonId}/active-delivery")]
        public async Task<ActionResult<DeliveryDetailDTO>> GetActiveDelivery(int deliveryPersonId)
        {
            var dp = await _deliveryPersonRepo.GetDeliveryPersonByIdAsync(deliveryPersonId);
            if (dp == null)
                return NotFound(new { message = "Delivery person not found" });

            var delivery = await _deliveryRepo.GetActiveDeliveryByPersonAsync(deliveryPersonId);
            if (delivery == null)
                return NotFound(new { message = "No active delivery found" });

            return Ok(delivery);
        }

        // GET: api/Delivery/{deliveryPersonId}/history
        [HttpGet("{deliveryPersonId}/history")]
        public async Task<ActionResult<IEnumerable<DeliveryDetailDTO>>> GetDeliveryHistory(int deliveryPersonId)
        {
            var dp = await _deliveryPersonRepo.GetDeliveryPersonByIdAsync(deliveryPersonId);
            if (dp == null)
                return NotFound(new { message = "Delivery person not found" });

            var deliveries = await _deliveryRepo.GetDeliveryHistoryAsync(deliveryPersonId);
            return Ok(deliveries);
        }

        // POST: api/Delivery/{deliveryPersonId}/accept/{orderId}
        [HttpPost("{deliveryPersonId}/accept/{orderId}")]
        public async Task<ActionResult<DeliveryDetailDTO>> AcceptDelivery(int deliveryPersonId, int orderId)
        {
            var dp = await _deliveryPersonRepo.GetDeliveryPersonByIdAsync(deliveryPersonId);
            if (dp == null)
                return NotFound(new { message = "Delivery person not found" });

            var hasActiveDelivery = await _deliveryRepo.HasActiveDeliveryAsync(deliveryPersonId);
            if (hasActiveDelivery)
                return BadRequest(new { message = "You already have an active delivery. Complete it before accepting another." });

            var order = await _orderRepo.GetOrderByIdAsync(orderId);
            if (order == null)
                return NotFound(new { message = "Order not found" });

            if (order.Status != OrderStatus.ACCEPTED)
                return BadRequest(new { message = "Order is not available for delivery" });

            var delivery = await _deliveryRepo.CreateDeliveryForOrderAsync(orderId, deliveryPersonId);
            if (delivery == null)
                return StatusCode(500, new { message = "Failed to create delivery" });

            return Ok(delivery);
        }

        // POST: api/Delivery/{deliveryPersonId}/pickup/{deliveryId}
        [HttpPost("{deliveryPersonId}/pickup/{deliveryId}")]
        public async Task<ActionResult<DeliveryDetailDTO>> MarkAsPickedUp(int deliveryPersonId, int deliveryId)
        {
            var dp = await _deliveryPersonRepo.GetDeliveryPersonByIdAsync(deliveryPersonId);
            if (dp == null)
                return NotFound(new { message = "Delivery person not found" });

            var delivery = await _deliveryRepo.GetDeliveryByIdAsync(deliveryId);
            if (delivery == null)
                return NotFound(new { message = "Delivery not found" });

            if (delivery.DeliveryPersonID != deliveryPersonId)
                return Forbid();

            if (delivery.Status != DeliveryStatus.ASSIGNED)
                return BadRequest(new { message = "Delivery must be in ASSIGNED status to pick up" });

            var updated = await _deliveryRepo.UpdateDeliveryStatusAsync(deliveryId, DeliveryStatus.PICKED_UP);
            if (updated == null)
                return StatusCode(500, new { message = "Failed to update delivery status" });

            return Ok(updated);
        }

        // POST: api/Delivery/{deliveryPersonId}/confirm-delivery/{deliveryId}
        [HttpPost("{deliveryPersonId}/confirm-delivery/{deliveryId}")]
        public async Task<ActionResult<DeliveryDetailDTO>> ConfirmDeliveryByPerson(int deliveryPersonId, int deliveryId)
        {
            var dp = await _deliveryPersonRepo.GetDeliveryPersonByIdAsync(deliveryPersonId);
            if (dp == null)
                return NotFound(new { message = "Delivery person not found" });

            var delivery = await _deliveryRepo.GetDeliveryByIdAsync(deliveryId);
            if (delivery == null)
                return NotFound(new { message = "Delivery not found" });

            if (delivery.DeliveryPersonID != deliveryPersonId)
                return Forbid();

            if (delivery.Status != DeliveryStatus.PICKED_UP)
                return BadRequest(new { message = "Delivery must be picked up before it can be confirmed." });

            var updated = await _deliveryRepo.ConfirmDeliveryByPersonAsync(deliveryId);
            if (updated == null)
                return BadRequest(new { message = "Failed to confirm delivery." });

            return Ok(updated);
        }

        // POST: api/Delivery/buyer-confirm/{deliveryId}
        [HttpPost("buyer-confirm/{deliveryId}")]
        public async Task<ActionResult<DeliveryDetailDTO>> ConfirmDeliveryByBuyer(int deliveryId, [FromQuery] int buyerId)
        {
            var delivery = await _deliveryRepo.GetDeliveryByIdAsync(deliveryId);
            if (delivery == null)
                return NotFound(new { message = "Delivery not found" });

            // Verify buyer owns this order
            var order = await _orderRepo.GetOrderByIdAsync(delivery.OrderID);
            if (order == null || order.BuyerID != buyerId)
                return Forbid();

            if (delivery.Status != DeliveryStatus.PICKED_UP)
                return BadRequest(new { message = "Delivery must be picked up before it can be confirmed." });

            var updated = await _deliveryRepo.ConfirmDeliveryByBuyerAsync(deliveryId);
            if (updated == null)
                return BadRequest(new { message = "Failed to confirm delivery." });

            return Ok(updated);
        }

        // POST: api/Delivery/{deliveryPersonId}/cancel/{deliveryId}
        [HttpPost("{deliveryPersonId}/cancel/{deliveryId}")]
        public async Task<ActionResult> CancelDelivery(int deliveryPersonId, int deliveryId)
        {
            var dp = await _deliveryPersonRepo.GetDeliveryPersonByIdAsync(deliveryPersonId);
            if (dp == null)
                return NotFound(new { message = "Delivery person not found" });

            var delivery = await _deliveryRepo.GetDeliveryByIdAsync(deliveryId);
            if (delivery == null)
                return NotFound(new { message = "Delivery not found" });

            if (delivery.DeliveryPersonID != deliveryPersonId)
                return Forbid();

            if (delivery.Status == DeliveryStatus.DELIVERED)
                return BadRequest(new { message = "Cannot cancel a completed delivery" });

            var success = await _deliveryRepo.CancelDeliveryAsync(deliveryId);
            if (!success)
                return StatusCode(500, new { message = "Failed to cancel delivery" });

            return Ok(new { message = "Delivery cancelled successfully" });
        }

        // GET: api/Delivery/{deliveryPersonId}/stats
        [HttpGet("{deliveryPersonId}/stats")]
        public async Task<ActionResult<DeliveryPersonStatsDTO>> GetDeliveryPersonStats(int deliveryPersonId)
        {
            var dp = await _deliveryPersonRepo.GetDeliveryPersonByIdAsync(deliveryPersonId);
            if (dp == null)
                return NotFound(new { message = "Delivery person not found" });

            var stats = await _deliveryRepo.GetDeliveryPersonStatsAsync(deliveryPersonId);
            return Ok(stats);
        }
    }
}