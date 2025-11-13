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
        // Get all available orders for delivery (ACCEPTED status, no active delivery for person)
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
        // Get current active delivery for delivery person
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
        // Get delivery history for delivery person
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
        // Delivery person accepts order → Order = DELIVERY_ASSIGNED, Delivery = ASSIGNED
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

            // Creates delivery with ASSIGNED status, changes order to DELIVERY_ASSIGNED
            var delivery = await _deliveryRepo.CreateDeliveryForOrderAsync(orderId, deliveryPersonId);
            if (delivery == null)
                return StatusCode(500, new { message = "Failed to create delivery" });

            return Ok(delivery);
        }

        // POST: api/Delivery/{deliveryPersonId}/pickup/{deliveryId}
        // Delivery person picks up → Order = DELIVERING, Delivery = PICKED_UP
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

            // Changes delivery to PICKED_UP and order to DELIVERING
            var updated = await _deliveryRepo.UpdateDeliveryStatusAsync(deliveryId, DeliveryStatus.PICKED_UP);
            if (updated == null)
                return StatusCode(500, new { message = "Failed to update delivery status" });

            return Ok(updated);
        }

        // POST: api/Delivery/{deliveryPersonId}/start-delivering/{deliveryId}
        // Start delivering (changes order status to DELIVERING)
        //[HttpPost("{deliveryPersonId}/start-delivering/{deliveryId}")]
        //public async Task<ActionResult<DeliveryDetailDTO>> StartDelivering(int deliveryPersonId, int deliveryId)
        //{
        //    var dp = await _deliveryPersonRepo.GetDeliveryPersonByIdAsync(deliveryPersonId);
        //    if (dp == null)
        //        return NotFound(new { message = "Delivery person not found" });

        //    var delivery = await _deliveryRepo.GetDeliveryByIdAsync(deliveryId);
        //    if (delivery == null)
        //        return NotFound(new { message = "Delivery not found" });

        //    if (delivery.DeliveryPersonID != deliveryPersonId)
        //        return Forbid();

        //    if (delivery.Status != DeliveryStatus.PICKED_UP)
        //        return BadRequest(new { message = "Order must be picked up first" });

        //    // Update order status to DELIVERING (delivery status stays PICKED_UP)
        //    var updated = await _deliveryRepo.StartDeliveringAsync(deliveryId);
        //    if (updated == null)
        //        return StatusCode(500, new { message = "Failed to start delivery" });

        //    return Ok(updated);
        //}

        // POST: api/Delivery/{deliveryPersonId}/confirm-delivery/{deliveryId}
        // Delivery person delivers and receives payment → Order = COMPLETED, Delivery = DELIVERED
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

            var order = await _orderRepo.GetOrderByIdAsync(delivery.OrderID);
            if (order == null || order.Status != OrderStatus.DELIVERING)
                return BadRequest(new { message = "Order must be in DELIVERING status before confirming delivery." });

            // Completes the delivery and order
            var updated = await _deliveryRepo.ConfirmDeliveryByPersonAsync(deliveryId);
            if (updated == null)
                return BadRequest(new { message = "Failed to confirm delivery. Ensure the order is in the correct status." });

            return Ok(updated);
        }

        // POST: api/Delivery/buyer-confirm/{deliveryId}
        // Buyer confirms delivery receipt
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

            if (order.Status != OrderStatus.DELIVERING)
                return BadRequest(new { message = "Order must be in DELIVERING status before confirming delivery." });

            var updated = await _deliveryRepo.ConfirmDeliveryByBuyerAsync(deliveryId);
            if (updated == null)
                return BadRequest(new { message = "Failed to confirm delivery. Ensure the order is in the correct status." });

            return Ok(updated);
        }

        // POST: api/Delivery/{deliveryPersonId}/cancel/{deliveryId}
        // Cancel delivery → Order = back to ACCEPTED, Delivery = FAILED
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
        // Get delivery person statistics
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