using ShelfLife.DTOs;
using ShelfLife.Models;

namespace ShelfLife.Repository.Base
{
    public interface IBookListingRepository
    {
        Task<IEnumerable<BookListingDisplayDTO>> GetAllListingsAsync(BookListingFilterDTO? filter = null);
        Task<BookListingDetailDTO?> GetListingByIdAsync(int listingId);
        Task<IEnumerable<BookListingDisplayDTO>> GetUserListingsAsync(int userId);
        Task<BookListing?> CreateListingAsync(BookListing listing);
        Task<BookListing?> UpdateListingAsync(BookListing listing);
        Task<bool> DeleteListingAsync(int listingId);
        Task<bool> UserOwnsListingAsync(int userId, int listingId);
        Task<int> GetTotalListingsCountAsync(BookListingFilterDTO? filter = null);

        // NEW: Added for order creation logic
        Task UpdateBookListingAsync(BookListing listing);
        Task<BookListing?> GetBookListingByIdAsync(int id);
    }
}