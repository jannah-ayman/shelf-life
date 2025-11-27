using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShelfLife.DTOs;
using ShelfLife.Models;
using ShelfLife.Repository;
using ShelfLife.Repository.Base;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ShelfLife.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepo;
        private readonly IBookListingRepository _listingRepo;
        private readonly IOrderRepository _orderRepo;

        public UserController(
            IUserRepository userRepo,
            IBookListingRepository listingRepo,
            IOrderRepository orderRepo)
        {
            _userRepo = userRepo;
            _listingRepo = listingRepo;
            _orderRepo = orderRepo;
        }

        // GET: api/User/{userId}/profile
        [HttpGet("{userId}/profile")]
        [Authorize(Policy = "UserMatchesRoute")]
        public async Task<ActionResult<UserProfileDTO>> GetUserProfile(int userId)
        {
            var profile = await _userRepo.GetUserProfileAsync(userId);
            if (profile == null)
                return NotFound(new { message = "User not found" });

            return Ok(profile);
        }

        // PUT: api/User/{userId}/profile
        [HttpPut("{userId}/profile")]
        [Authorize(Policy = "UserMatchesRoute")]
        public async Task<ActionResult<UserProfileDTO>> UpdateUserProfile(int userId, [FromBody] UpdateUserProfileDTO updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userRepo.GetUserByIdAsync(userId);
            if (user == null)
                return NotFound(new { message = "User not found" });

            // Update user properties
            user.Email = updateDto.Email;
            user.Phone = updateDto.Phone;
            user.Name = updateDto.Name;
            user.Address = updateDto.Address;
            user.City = updateDto.City;
            user.ProfilePhotoURL = updateDto.ProfilePhotoURL;

            var updatedUser = await _userRepo.UpdateUserAsync(user);
            if (updatedUser == null)
                return StatusCode(500, new { message = "Failed to update profile" });

            var profile = await _userRepo.GetUserProfileAsync(userId);
            return Ok(profile);
        }

        // DELETE: api/User/{userId}
        [HttpDelete("{userId}")]
        [Authorize(Policy = "UserMatchesRoute")]
        public async Task<ActionResult> DeleteUserAccount(int userId)
        {
            var user = await _userRepo.GetUserByIdAsync(userId);
            if (user == null)
                return NotFound(new { message = "User not found" });

            var success = await _userRepo.DeleteUserAsync(userId);
            if (!success)
                return StatusCode(500, new { message = "Failed to delete account" });

            return Ok(new { message = "Account deleted successfully" });
        }

        // POST: api/User/{userId}/change-password
        [HttpPost("{userId}/change-password")]
        [Authorize(Policy = "UserMatchesRoute")]
        public async Task<ActionResult> ChangePassword(int userId, [FromBody] ChangePasswordDTO passwordDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // FIXED: Hash the passwords before comparing/storing
            var currentPasswordHash = HashPassword(passwordDto.CurrentPassword);
            var newPasswordHash = HashPassword(passwordDto.NewPassword);

            var success = await _userRepo.ChangePasswordAsync(userId, currentPasswordHash, newPasswordHash);
            if (!success)
                return BadRequest(new { message = "Current password is incorrect" });

            return Ok(new { message = "Password changed successfully" });
        }

        // GET: api/User/{userId}/stats
        [HttpGet("{userId}/stats")]
        [Authorize(Policy = "UserMatchesRoute")]
        public async Task<ActionResult<UserDashboardStatsDTO>> GetDashboardStats(int userId)
        {
            var stats = await _userRepo.GetDashboardStatsAsync(userId);
            return Ok(stats);
        }

        // GET: api/User/{userId}/listings
        [HttpGet("{userId}/listings")]
        [Authorize(Policy = "UserMatchesRoute")]
        public async Task<ActionResult<IEnumerable<BookListingDisplayDTO>>> GetUserListings(int userId)
        {
            var listings = await _listingRepo.GetUserListingsAsync(userId);
            return Ok(listings);
        }

        // GET: api/User/{userId}/listings/{listingId}
        [HttpGet("{userId}/listings/{listingId}")]
        [Authorize(Policy = "UserMatchesRoute")]
        public async Task<ActionResult<BookListingDetailDTO>> GetListingDetail(int userId, int listingId)
        {
            var ownsListing = await _listingRepo.UserOwnsListingAsync(userId, listingId);
            if (!ownsListing)
                return Forbid();

            var listing = await _listingRepo.GetListingByIdAsync(listingId);
            if (listing == null)
                return NotFound(new { message = "Listing not found" });

            return Ok(listing);
        }

        // POST: api/User/{userId}/listings
        [HttpPost("{userId}/listings")]
        [Authorize(Policy = "UserMatchesRoute")]
        public async Task<ActionResult<BookListingDetailDTO>> CreateListing(int userId, [FromBody] CreateBookListingDTO createDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userRepo.GetUserByIdAsync(userId);
            if (user == null)
                return NotFound(new { message = "User not found" });

            // --- ENFORCE RULES ---

            // Rule 1: only normal users can swap
            if (createDto.IsSwappable && user.UserType != UserType.NORMAL_USER)
                return BadRequest(new { message = "Only normal users can enable swapping." });

            // Rule 2: quantity limit for normal users
            if (user.UserType == UserType.NORMAL_USER && createDto.Quantity > 1)
                return BadRequest(new { message = "Normal users can only list a quantity of 1." });

            // Validate: at least one transaction type
            if (!createDto.IsSellable && !createDto.IsSwappable)
                return BadRequest(new { message = "At least one transaction type (Sellable or Swappable) must be enabled" });

            // Validate: price required for sellable items
            if (createDto.IsSellable && !createDto.Price.HasValue)
                return BadRequest(new { message = "Price is required for sellable items" });

            var listing = new BookListing
            {
                UserID = userId,
                Title = createDto.Title,
                Author = createDto.Author,
                ISBN = createDto.ISBN,
                CategoryID = createDto.CategoryID,
                Condition = createDto.Condition,
                Price = createDto.Price,
                Edition = createDto.Edition,
                Description = createDto.Description,
                PhotoURLs = createDto.PhotoURLs,
                City = createDto.City ?? user.City,
                IsSellable = createDto.IsSellable,
                IsSwappable = createDto.IsSwappable,
                Quantity = createDto.Quantity,
                AvailableQuantity = createDto.Quantity,
                AvailabilityStatus = AvailabilityStatus.Available,
                CreatedAt = DateTime.UtcNow
            };

            var createdListing = await _listingRepo.CreateListingAsync(listing);
            if (createdListing == null)
                return StatusCode(500, new { message = "Failed to create listing" });

            var listingDetail = await _listingRepo.GetListingByIdAsync(createdListing.BookListingID);
            return CreatedAtAction(nameof(GetListingDetail),
                new { userId, listingId = createdListing.BookListingID },
                listingDetail);
        }

        // PUT: api/User/{userId}/listings/{listingId}
        [HttpPut("{userId}/listings/{listingId}")]
        [Authorize(Policy = "UserMatchesRoute")]
        public async Task<ActionResult<BookListingDetailDTO>> UpdateListing(int userId, int listingId, [FromBody] UpdateBookListingDTO updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (listingId != updateDto.BookListingID)
                return BadRequest(new { message = "Listing ID mismatch" });

            var ownsListing = await _listingRepo.UserOwnsListingAsync(userId, listingId);
            if (!ownsListing)
                return Forbid();

            var listingEntity = await _userRepo.GetUserByIdAsync(userId);
            if (listingEntity == null)
                return NotFound(new { message = "User not found" });

            var listing = listingEntity.BookListings.FirstOrDefault(l => l.BookListingID == listingId);
            if (listing == null)
                return NotFound(new { message = "Listing not found" });

            // --- ENFORCE RULES ---
            if (listingEntity.UserType != UserType.NORMAL_USER && updateDto.IsSwappable)
                return BadRequest(new { message = "Only normal users can enable swapping." });

            if (listingEntity.UserType == UserType.NORMAL_USER && updateDto.Quantity > 1)
                return BadRequest(new { message = "Normal users can only list a quantity of 1." });

            // Validate: at least one transaction type must be enabled
            if (!updateDto.IsSellable && !updateDto.IsSwappable)
                return BadRequest(new { message = "At least one transaction type must be enabled" });

            // Validate: price is required for sellable items
            if (updateDto.IsSellable && !updateDto.Price.HasValue)
                return BadRequest(new { message = "Price is required for sellable items" });

            // Update properties
            listing.Title = updateDto.Title;
            listing.Author = updateDto.Author;
            listing.ISBN = updateDto.ISBN;
            listing.CategoryID = updateDto.CategoryID;
            listing.Condition = updateDto.Condition;
            listing.Price = updateDto.Price;
            listing.Edition = updateDto.Edition;
            listing.Description = updateDto.Description;
            listing.PhotoURLs = updateDto.PhotoURLs;
            listing.City = updateDto.City;
            listing.IsSellable = updateDto.IsSellable;
            listing.IsSwappable = updateDto.IsSwappable;
            listing.Quantity = updateDto.Quantity;
            listing.AvailabilityStatus = updateDto.AvailabilityStatus;

            // Recalculate available quantity if total quantity changed
            var soldQuantity = listing.Quantity - listing.AvailableQuantity;
            listing.AvailableQuantity = Math.Max(0, updateDto.Quantity - soldQuantity);

            var updatedListing = await _listingRepo.UpdateListingAsync(listing);
            if (updatedListing == null)
                return StatusCode(500, new { message = "Failed to update listing" });

            var listingDetail = await _listingRepo.GetListingByIdAsync(listingId);
            return Ok(listingDetail);
        }

        // DELETE: api/User/{userId}/listings/{listingId}
        [HttpDelete("{userId}/listings/{listingId}")]
        [Authorize(Policy = "UserMatchesRoute")]
        public async Task<ActionResult> DeleteListing(int userId, int listingId)
        {
            var ownsListing = await _listingRepo.UserOwnsListingAsync(userId, listingId);
            if (!ownsListing)
                return Forbid();

            var success = await _listingRepo.DeleteListingAsync(listingId);
            if (!success)
                return NotFound(new { message = "Listing not found" });

            return Ok(new { message = "Listing deleted successfully" });
        }

        // GET: api/User/{userId}/orders/incoming
        [HttpGet("{userId}/orders/incoming")]
        [Authorize(Policy = "UserMatchesRoute")]
        public async Task<ActionResult<IEnumerable<OrderDisplayDTO>>> GetIncomingOrders(int userId)
        {
            var orders = await _orderRepo.GetUserIncomingOrdersAsync(userId);
            return Ok(orders);
        }

        // GET: api/User/{userId}/orders/outgoing
        [HttpGet("{userId}/orders/outgoing")]
        [Authorize(Policy = "UserMatchesRoute")]
        public async Task<ActionResult<IEnumerable<OrderDisplayDTO>>> GetOutgoingOrders(int userId)
        {
            var orders = await _orderRepo.GetUserOutgoingOrdersAsync(userId);
            return Ok(orders);
        }

        // GET: api/User/{userId}/orders/{orderId}
        [HttpGet("{userId}/orders/{orderId}")]
        [Authorize(Policy = "UserMatchesRoute")]
        public async Task<ActionResult<OrderDisplayDTO>> GetOrderDetail(int userId, int orderId)
        {
            var ownsListing = await _orderRepo.UserOwnsOrderListingAsync(userId, orderId);
            var isBuyer = await _orderRepo.UserIsOrderBuyerAsync(userId, orderId);

            if (!ownsListing && !isBuyer)
                return Forbid();

            var order = await _orderRepo.GetOrderByIdAsync(orderId);
            if (order == null)
                return NotFound(new { message = "Order not found" });

            return Ok(order);
        }

        // Update UserController.cs - Add this method to check user type

        [HttpGet("{userId}/check-access")]
        public async Task<ActionResult> CheckUserAccess(int userId)
        {
            var user = await _userRepo.GetUserByIdAsync(userId);
            if (user == null)
                return NotFound(new { message = "User not found" });

            var userTypeFromToken = User.FindFirst(ClaimTypes.Role)?.Value;

            return Ok(new
            {
                userType = user.UserType.ToString(),
                isAuthorized = userTypeFromToken == user.UserType.ToString()
            });
        }

        // For operations that should be Business only
        [HttpGet("{userId}/business-stats")]
        [Authorize(Policy = "BusinessOnly")]
        public async Task<ActionResult> GetBusinessStats(int userId)
        {
            // Business-only operations
            return Ok(new { message = "Business stats" });
        }

        // For operations that should be Normal User only
        [HttpGet("{userId}/swap-listings")]
        [Authorize(Policy = "NormalUserOnly")]
        public async Task<ActionResult> GetSwapListings(int userId)
        {
            // Normal user-only operations
            return Ok(new { message = "Swap listings" });
        }

        // HELPER: Hash password using SHA256 (same as AuthController)
        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hash);
        }
    }
}