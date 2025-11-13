using Microsoft.AspNetCore.Mvc;
using ShelfLife.DTOs;
using ShelfLife.Models;
using ShelfLife.Repository.Base;

namespace ShelfLife.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderRepository _orderRepo;

        public OrdersController(IOrderRepository orderRepo)
        {
            _orderRepo = orderRepo;
        }

        // POST: api/Orders/sale
        // Sale orders are auto-accepted (ACCEPTED status immediately)
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

            var orderDisplay = await _orderRepo.GetOrderByIdAsync(order.OrderID);
            return CreatedAtAction(nameof(GetOrderById), new { id = order.OrderID }, orderDisplay);
        }

        // POST: api/Orders/swap
        // Swap orders start with NEGOTIATING status
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

            var orderDisplay = await _orderRepo.GetOrderByIdAsync(order.OrderID);
            return CreatedAtAction(nameof(GetOrderById), new { id = order.OrderID }, orderDisplay);
        }

        // POST: api/Orders/{orderId}/respond-swap
        // Seller accepts or rejects swap order
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

            // Verify seller owns the listing
            if (order.SellerID != sellerId)
                return Forbid();

            if (order.OrderType != OrderType.SWAP)
                return BadRequest(new { message = "This endpoint is only for swap orders" });

            if (order.Status != OrderStatus.NEGOTIATING)
                return BadRequest(new { message = "Order is not in negotiating status" });

            var success = await _orderRepo.RespondToSwapAsync(orderId, response.Accept);
            if (!success)
                return StatusCode(500, new { message = "Failed to respond to swap" });

            var updatedOrder = await _orderRepo.GetOrderByIdAsync(orderId);
            return Ok(updatedOrder);
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