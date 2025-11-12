using Microsoft.AspNetCore.Mvc;
using ShelfLife.DTOs;
using ShelfLife.Repository;
using ShelfLife.Repository.Base;

namespace ShelfLife.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ListingsController : ControllerBase
    {
        private readonly IBookListingRepository _listingRepo;

        public ListingsController(IBookListingRepository listingRepo)
        {
            _listingRepo = listingRepo;
        }

        // GET: api/Listings
        // Public endpoint for browsing all listings with filters
        [HttpGet]
        public async Task<ActionResult<object>> GetAllListings([FromQuery] BookListingFilterDTO filter)
        {
            var listings = await _listingRepo.GetAllListingsAsync(filter);
            var totalCount = await _listingRepo.GetTotalListingsCountAsync(filter);

            var totalPages = (int)Math.Ceiling((double)totalCount / filter.PageSize);

            return Ok(new
            {
                data = listings,
                pagination = new
                {
                    currentPage = filter.PageNumber,
                    pageSize = filter.PageSize,
                    totalCount = totalCount,
                    totalPages = totalPages,
                    hasNextPage = filter.PageNumber < totalPages,
                    hasPreviousPage = filter.PageNumber > 1
                }
            });
        }

        // GET: api/Listings/{id}
        // Public endpoint for viewing a single listing detail
        [HttpGet("{id}")]
        public async Task<ActionResult<BookListingDetailDTO>> GetListingById(int id)
        {
            var listing = await _listingRepo.GetListingByIdAsync(id);
            if (listing == null)
                return NotFound(new { message = "Listing not found" });

            return Ok(listing);
        }

        // GET: api/Listings/search
        // Alternative search endpoint with query string
        [HttpGet("search")]
        public async Task<ActionResult<object>> SearchListings(
            [FromQuery] string? searchTerm,
            [FromQuery] int? categoryId,
            [FromQuery] string? condition,
            [FromQuery] decimal? minPrice,
            [FromQuery] decimal? maxPrice,
            [FromQuery] string? city,
            [FromQuery] bool? isSellable,
            [FromQuery] bool? isSwappable,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string? sortBy = null,
            [FromQuery] bool sortDescending = false)
        {
            var filter = new BookListingFilterDTO
            {
                SearchTerm = searchTerm,
                CategoryID = categoryId,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                City = city,
                IsSellable = isSellable,
                IsSwappable = isSwappable,
                PageNumber = pageNumber,
                PageSize = pageSize,
                SortBy = sortBy,
                SortDescending = sortDescending
            };

            // Parse condition enum if provided
            if (!string.IsNullOrWhiteSpace(condition) &&
                Enum.TryParse<Models.BookCondition>(condition, true, out var conditionEnum))
            {
                filter.Condition = conditionEnum;
            }

            var listings = await _listingRepo.GetAllListingsAsync(filter);
            var totalCount = await _listingRepo.GetTotalListingsCountAsync(filter);

            var totalPages = (int)Math.Ceiling((double)totalCount / filter.PageSize);

            return Ok(new
            {
                data = listings,
                pagination = new
                {
                    currentPage = filter.PageNumber,
                    pageSize = filter.PageSize,
                    totalCount = totalCount,
                    totalPages = totalPages,
                    hasNextPage = filter.PageNumber < totalPages,
                    hasPreviousPage = filter.PageNumber > 1
                },
                filters = new
                {
                    searchTerm = filter.SearchTerm,
                    categoryId = filter.CategoryID,
                    condition = filter.Condition?.ToString(),
                    minPrice = filter.MinPrice,
                    maxPrice = filter.MaxPrice,
                    city = filter.City,
                    isSellable = filter.IsSellable,
                    isSwappable = filter.IsSwappable,
                    sortBy = filter.SortBy,
                    sortDescending = filter.SortDescending
                }
            });
        }

        // GET: api/Listings/cities
        // Get list of all cities with available listings
        [HttpGet("cities")]
        public async Task<ActionResult<IEnumerable<string>>> GetAvailableCities()
        {
            return Ok(new[] { "Cairo", "Alexandria", "Giza", "Assiut" });
        }
    }
}