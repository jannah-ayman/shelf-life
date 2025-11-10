using Microsoft.AspNetCore.Mvc;
using ShelfLife.DTOs;
using ShelfLife.Models;
using ShelfLife.Repository;
using ShelfLife.Repository.Base;

namespace ShelfLife.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserDashboardController : ControllerBase
    {
        private readonly IUserRepository _userRepo;
        private readonly IBookListingRepository _listingRepo;
        private readonly IOrderRepository _orderRepo;

        public UserDashboardController(
            IUserRepository userRepo,
            IBookListingRepository listingRepo,
            IOrderRepository orderRepo)
        {
            _userRepo = userRepo;
            _listingRepo = listingRepo;
            _orderRepo = orderRepo;
        }

        // GET: api/UserDashboard/{userId}/profile
        [HttpGet("{userId}/profile")]
        public async Task<ActionResult<UserProfileDTO>> GetUserProfile(int userId)
        {
            var profile = await _userRepo.GetUserProfileAsync(userId);
            if (profile == null)
                return NotFound(new { message = "User not found" });

            return Ok(profile);
        }

        // PUT: api/UserDashboard/{userId}/profile
        [HttpPut("{userId}/profile")]
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

        // DELETE: api/UserDashboard/{userId}
        [HttpDelete("{userId}")]
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

        // POST: api/UserDashboard/{userId}/change-password
        [HttpPost("{userId}/change-password")]
        public async Task<ActionResult> ChangePassword(int userId, [FromBody] ChangePasswordDTO passwordDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // NOTE: In production, implement proper password hashing (BCrypt, etc.)
            // This is simplified for demonstration
            var currentPasswordHash = passwordDto.CurrentPassword; // Should be hashed
            var newPasswordHash = passwordDto.NewPassword; // Should be hashed

            var success = await _userRepo.ChangePasswordAsync(userId, currentPasswordHash, newPasswordHash);
            if (!success)
                return BadRequest(new { message = "Current password is incorrect" });

            return Ok(new { message = "Password changed successfully" });
        }

        // GET: api/UserDashboard/{userId}/stats
        [HttpGet("{userId}/stats")]
        public async Task<ActionResult<UserDashboardStatsDTO>> GetDashboardStats(int userId)
        {
            var stats = await _userRepo.GetDashboardStatsAsync(userId);
            return Ok(stats);
        }

        // GET: api/UserDashboard/{userId}/listings
        [HttpGet("{userId}/listings")]
        public async Task<ActionResult<IEnumerable<BookListingDisplayDTO>>> GetUserListings(int userId)
        {
            var listings = await _listingRepo.GetUserListingsAsync(userId);
            return Ok(listings);
        }

        // GET: api/UserDashboard/{userId}/listings/{listingId}
        [HttpGet("{userId}/listings/{listingId}")]
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

        // POST: api/UserDashboard/{userId}/listings
        [HttpPost("{userId}/listings")]
        public async Task<ActionResult<BookListingDetailDTO>> CreateListing(int userId, [FromBody] CreateBookListingDTO createDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userRepo.GetUserByIdAsync(userId);
            if (user == null)
                return NotFound(new { message = "User not found" });

            // Validate: at least one transaction type must be enabled
            if (!createDto.IsSellable && !createDto.IsDonatable && !createDto.IsSwappable)
                return BadRequest(new { message = "At least one transaction type (Sellable, Donatable, Swappable) must be enabled" });

            // Validate: price is required for sellable items
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
                IsDonatable = createDto.IsDonatable,
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

        // PUT: api/UserDashboard/{userId}/listings/{listingId}
        [HttpPut("{userId}/listings/{listingId}")]
        public async Task<ActionResult<BookListingDetailDTO>> UpdateListing(int userId, int listingId, [FromBody] UpdateBookListingDTO updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (listingId != updateDto.BookListingID)
                return BadRequest(new { message = "Listing ID mismatch" });

            var ownsListing = await _listingRepo.UserOwnsListingAsync(userId, listingId);
            if (!ownsListing)
                return Forbid();

            var existingListing = await _listingRepo.GetListingByIdAsync(listingId);
            if (existingListing == null)
                return NotFound(new { message = "Listing not found" });

            // Validate: at least one transaction type must be enabled
            if (!updateDto.IsSellable && !updateDto.IsDonatable && !updateDto.IsSwappable)
                return BadRequest(new { message = "At least one transaction type must be enabled" });

            // Validate: price is required for sellable items
            if (updateDto.IsSellable && !updateDto.Price.HasValue)
                return BadRequest(new { message = "Price is required for sellable items" });

            // Get the listing entity to update
            var listingEntity = await _userRepo.GetUserByIdAsync(userId);
            var listing = listingEntity?.BookListings.FirstOrDefault(l => l.BookListingID == listingId);

            if (listing == null)
                return NotFound(new { message = "Listing not found" });

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
            listing.IsDonatable = updateDto.IsDonatable;
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

        // DELETE: api/UserDashboard/{userId}/listings/{listingId}
        [HttpDelete("{userId}/listings/{listingId}")]
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

        // GET: api/UserDashboard/{userId}/orders/incoming
        [HttpGet("{userId}/orders/incoming")]
        public async Task<ActionResult<IEnumerable<OrderDisplayDTO>>> GetIncomingOrders(int userId)
        {
            var orders = await _orderRepo.GetUserIncomingOrdersAsync(userId);
            return Ok(orders);
        }

        // GET: api/UserDashboard/{userId}/orders/outgoing
        [HttpGet("{userId}/orders/outgoing")]
        public async Task<ActionResult<IEnumerable<OrderDisplayDTO>>> GetOutgoingOrders(int userId)
        {
            var orders = await _orderRepo.GetUserOutgoingOrdersAsync(userId);
            return Ok(orders);
        }

        // GET: api/UserDashboard/{userId}/orders/{orderId}
        [HttpGet("{userId}/orders/{orderId}")]
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
    }
}