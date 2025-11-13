using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShelfLife.DTOs;
using ShelfLife.Models;
using ShelfLife.Repository.Base;

namespace ShelfLife.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RatingsController : ControllerBase
    {
        private readonly IRatingRepository _ratingRepo;
        private readonly IOrderRepository _orderRepo;

        public RatingsController(IRatingRepository ratingRepo, IOrderRepository orderRepo)
        {
            _ratingRepo = ratingRepo;
            _orderRepo = orderRepo;
        }

        // POST: api/Ratings
        [HttpPost]
        public async Task<IActionResult> CreateRating([FromBody] CreateRatingDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var order = await _orderRepo.GetOrderByIdAsync(dto.OrderID);
            if (order == null)
                return NotFound(new { message = "Order not found" });

            if (order.Status != OrderStatus.COMPLETED)
                return BadRequest(new { message = "Order must be completed before rating" });

            var rating = await _ratingRepo.CreateRatingAsync(dto);
            if (rating == null)
                return BadRequest(new { message = "Rating already exists or order invalid" });

            var ratingDisplay = await _ratingRepo.GetRatingByOrderIdAsync(dto.OrderID);
            return Ok(ratingDisplay);
        }

        // GET: api/Ratings/order/{orderId}
        [HttpGet("order/{orderId}")]
        public async Task<IActionResult> GetRatingByOrder(int orderId)
        {
            var rating = await _ratingRepo.GetRatingByOrderIdAsync(orderId);
            if (rating == null)
                return NotFound(new { message = "Rating not found" });

            return Ok(rating);
        }

        // GET: api/Ratings/user/{userId}
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserRatings(int userId)
        {
            var ratings = await _ratingRepo.GetUserRatingsAsync(userId);
            return Ok(ratings);
        }
    }
}
