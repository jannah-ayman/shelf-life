using Microsoft.AspNetCore.Mvc;
using ShelfLife.DTOs;
using ShelfLife.Hubs;
using ShelfLife.Models;
using ShelfLife.Repository.Base;

namespace ShelfLife.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderRepository _orderRepo;
        private readonly NotificationService _notificationService;

        public OrdersController(IOrderRepository orderRepo, NotificationService notificationService)
        {
            _orderRepo = orderRepo;
            _notificationService = notificationService;
        }

        // POST: api/Orders/sale
        [HttpPost("sale")]
        public async Task<ActionResult<OrderDisplayDTO>> CreateSaleOrder([FromQuery] int buyerId, [FromBody] CreateSaleOrderDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var buyer = await _orderRepo.GetUserByIdAsync(buyerId);
            if (buyer == null)
                return NotFound(new { message = "Buyer not found." });

            var listing = await _orderRepo.GetListingByIdAsync(dto.ListingID);
            if (listing == null)
                return NotFound(new { message = "Listing not found." });

            var seller = listing.User;
            if (seller == null)
                return StatusCode(500, new { message = "Listing owner information is unavailable." });

            if (buyer.UserType == UserType.BUSINESS && seller.UserType == UserType.NORMAL_USER)
                return StatusCode(403, new { message = "Business users cannot purchase listings from normal users." });

            if (!listing.IsSellable)
                return BadRequest(new { message = "This listing is not sellable." });

            if (listing.UserID == buyerId)
                return BadRequest(new { message = "You cannot buy your own listing." });

            if (listing.AvailableQuantity < dto.Quantity)
                return BadRequest(new { message = "Not enough quantity available." });

            var order = await _orderRepo.CreateSaleOrderAsync(buyerId, dto);
            if (order == null)
                return BadRequest(new { message = "Unable to create sale order." });

            // Send notification to seller
            await _notificationService.SendNotificationAsync(
                seller.UserID,
                NotificationType.ORDER_RECEIVED,
                "New Order Received",
                $"{buyer.Name} placed an order for your book: {listing.Title}"
            );

            var orderDisplay = await _orderRepo.GetOrderByIdAsync(order.OrderID);
            return CreatedAtAction(nameof(GetOrderById), new { id = order.OrderID }, orderDisplay);
        }

        // POST: api/Orders/swap
        [HttpPost("swap")]
        public async Task<ActionResult<OrderDisplayDTO>> CreateSwapOrder([FromQuery] int buyerId, [FromBody] CreateSwapOrderDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!await _orderRepo.CanUserSwapAsync(buyerId))
                return StatusCode(403, new { message = "Only normal users can create swap orders. Business users cannot swap." });

            var order = await _orderRepo.CreateSwapOrderAsync(buyerId, dto);
            if (order == null)
                return BadRequest(new { message = "Unable to create swap order. Please check listings and ownership rules." });

            // Get listing and send notification
            var listing = await _orderRepo.GetListingByIdAsync(dto.ListingID);
            var buyer = await _orderRepo.GetUserByIdAsync(buyerId);
            if (listing != null && buyer != null)
            {
                await _notificationService.SendNotificationAsync(
                    listing.UserID,
                    NotificationType.NEGOTIATION_MESSAGE,
                    "New Swap Request",
                    $"{buyer.Name} wants to swap for your book: {listing.Title}"
                );
            }

            var orderDisplay = await _orderRepo.GetOrderByIdAsync(order.OrderID);
            return CreatedAtAction(nameof(GetOrderById), new { id = order.OrderID }, orderDisplay);
        }

        // POST: api/Orders/{orderId}/respond-swap
        [HttpPost("{orderId}/respond-swap")]
        public async Task<ActionResult<OrderDisplayDTO>> RespondToSwap(
            int orderId,
            [FromQuery] int sellerId,
            [FromBody] SwapResponseDTO response)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var order = await _orderRepo.GetOrderByIdAsync(orderId);
            if (order == null)
                return NotFound(new { message = "Order not found" });

            if (order.SellerID != sellerId)
                return Forbid();

            if (order.OrderType != OrderType.SWAP)
                return BadRequest(new { message = "This endpoint is only for swap orders" });

            if (order.Status != OrderStatus.NEGOTIATING)
                return BadRequest(new { message = "Order is not in negotiating status" });

            var success = await _orderRepo.RespondToSwapAsync(orderId, response.Accept);
            if (!success)
                return StatusCode(500, new { message = "Failed to respond to swap" });

            // Send notification to buyer
            var listing = await _orderRepo.GetListingByIdAsync(order.ListingID);
            var seller = await _orderRepo.GetUserByIdAsync(sellerId);

            if (response.Accept)
            {
                await _notificationService.SendNotificationAsync(
                    order.BuyerID,
                    NotificationType.ORDER_RECEIVED,
                    "Swap Request Accepted",
                    $"{seller?.Name} accepted your swap request for: {listing?.Title}"
                );
            }
            else
            {
                await _notificationService.SendNotificationAsync(
                    order.BuyerID,
                    NotificationType.ORDER_CANCELLED,
                    "Swap Request Rejected",
                    $"{seller?.Name} rejected your swap request for: {listing?.Title}"
                );
            }

            var updatedOrder = await _orderRepo.GetOrderByIdAsync(orderId);
            return Ok(updatedOrder);
        }

        // POST: api/Orders/{orderId}/cancel
        [HttpPost("{orderId}/cancel")]
        public async Task<ActionResult> CancelOrder(int orderId, [FromQuery] int userId)
        {
            var order = await _orderRepo.GetOrderByIdAsync(orderId);
            if (order == null)
                return NotFound(new { message = "Order not found" });

            // Only buyer or seller can cancel
            bool isBuyer = order.BuyerID == userId;
            bool isSeller = order.SellerID == userId;

            if (!isBuyer && !isSeller)
                return Forbid();

            // Cannot cancel if delivery is in progress or completed
            if (order.Status == OrderStatus.DELIVERING || order.Status == OrderStatus.COMPLETED)
                return BadRequest(new { message = "Cannot cancel order at this stage" });

            // If delivery is assigned, need to cancel delivery first
            if (order.Status == OrderStatus.DELIVERY_ASSIGNED)
                return BadRequest(new { message = "Cannot cancel order with assigned delivery. Contact delivery person first." });

            var success = await _orderRepo.CancelOrderAsync(orderId);
            if (!success)
                return StatusCode(500, new { message = "Failed to cancel order" });

            // Notify the other party
            int notifyUserId = isBuyer ? order.SellerID : order.BuyerID;
            var cancelledBy = await _orderRepo.GetUserByIdAsync(userId);
            var listing = await _orderRepo.GetListingByIdAsync(order.ListingID);

            await _notificationService.SendNotificationAsync(
                notifyUserId,
                NotificationType.ORDER_CANCELLED,
                "Order Cancelled",
                $"{cancelledBy?.Name} cancelled the order for: {listing?.Title}"
            );

            return Ok(new { message = "Order cancelled successfully" });
        }

        // GET: api/Orders/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDisplayDTO>> GetOrderById(int id)
        {
            var order = await _orderRepo.GetOrderByIdAsync(id);
            if (order == null)
                return NotFound(new { message = "Order not found" });

            return Ok(order);
        }

        // GET: api/Orders/{id}/payment-breakdown
        [HttpGet("{id}/payment-breakdown")]
        public async Task<ActionResult<PaymentBreakdownDTO>> GetPaymentBreakdown(int id)
        {
            var breakdown = await _orderRepo.GetPaymentBreakdownAsync(id);
            if (breakdown == null)
                return NotFound(new { message = "Order not found" });

            return Ok(breakdown);
        }

        // GET: api/Orders/incoming/{userId}
        [HttpGet("incoming/{userId}")]
        public async Task<ActionResult<IEnumerable<OrderDisplayDTO>>> GetIncomingOrders(int userId)
        {
            var orders = await _orderRepo.GetUserIncomingOrdersAsync(userId);
            return Ok(orders);
        }

        // GET: api/Orders/outgoing/{userId}
        [HttpGet("outgoing/{userId}")]
        public async Task<ActionResult<IEnumerable<OrderDisplayDTO>>> GetOutgoingOrders(int userId)
        {
            var orders = await _orderRepo.GetUserOutgoingOrdersAsync(userId);
            return Ok(orders);
        }

        // DELETE: api/Orders/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var success = await _orderRepo.DeleteOrderAsync(id);
            if (!success)
                return NotFound(new { message = "Order not found" });
            return NoContent();
        }
    }
}