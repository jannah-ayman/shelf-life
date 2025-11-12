using Microsoft.EntityFrameworkCore;
using ShelfLife.DTOs;
using ShelfLife.Models;
using ShelfLife.Repository.Base;

namespace ShelfLife.Repository
{
    public class BookListingRepository : IBookListingRepository
    {
        private readonly DBcontext _context;

        public BookListingRepository(DBcontext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BookListingDisplayDTO>> GetAllListingsAsync(BookListingFilterDTO? filter = null)
        {
            var query = _context.BookListings
                .Include(b => b.User)
                .Include(b => b.Category)
                .Where(b => b.AvailableQuantity > 0)
                .AsQueryable();

            // Apply filters
            if (filter != null)
            {
                if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
                {
                    var searchLower = filter.SearchTerm.ToLower();
                    query = query.Where(b =>
                        b.Title.ToLower().Contains(searchLower) ||
                        (b.Author != null && b.Author.ToLower().Contains(searchLower)) ||
                        (b.ISBN != null && b.ISBN.Contains(searchLower))
                    );
                }

                if (filter.CategoryID.HasValue)
                    query = query.Where(b => b.CategoryID == filter.CategoryID.Value);

                if (filter.Condition.HasValue)
                    query = query.Where(b => b.Condition == filter.Condition.Value);

                if (filter.MinPrice.HasValue)
                    query = query.Where(b => b.Price >= filter.MinPrice.Value);

                if (filter.MaxPrice.HasValue)
                    query = query.Where(b => b.Price <= filter.MaxPrice.Value);

                if (!string.IsNullOrWhiteSpace(filter.City))
                    query = query.Where(b => b.City == filter.City);

                if (filter.IsSellable.HasValue)
                    query = query.Where(b => b.IsSellable == filter.IsSellable.Value);

                if (filter.IsSwappable.HasValue)
                    query = query.Where(b => b.IsSwappable == filter.IsSwappable.Value);

                // Sorting
                query = filter.SortBy?.ToLower() switch
                {
                    "price" => filter.SortDescending
                        ? query.OrderByDescending(b => b.Price)
                        : query.OrderBy(b => b.Price),
                    "title" => filter.SortDescending
                        ? query.OrderByDescending(b => b.Title)
                        : query.OrderBy(b => b.Title),
                    "date" => filter.SortDescending
                        ? query.OrderByDescending(b => b.CreatedAt)
                        : query.OrderBy(b => b.CreatedAt),
                    _ => query.OrderByDescending(b => b.CreatedAt)
                };

                // Pagination
                var skip = (filter.PageNumber - 1) * filter.PageSize;
                query = query.Skip(skip).Take(filter.PageSize);
            }
            else
            {
                query = query.OrderByDescending(b => b.CreatedAt);
            }

            return await query.Select(b => new BookListingDisplayDTO
            {
                BookListingID = b.BookListingID,
                Title = b.Title,
                Author = b.Author,
                ISBN = b.ISBN,
                Condition = b.Condition,
                Price = b.Price,
                CategoryName = b.Category != null ? b.Category.Name : null,
                City = b.City,
                IsSellable = b.IsSellable,
                IsSwappable = b.IsSwappable,
                AvailableQuantity = b.AvailableQuantity,
                AvailabilityStatus = b.AvailabilityStatus,
                PhotoURLs = b.PhotoURLs,
                CreatedAt = b.CreatedAt,
                UserID = b.UserID,
                OwnerName = b.User.Name,
                OwnerRating = b.User.AverageRating
            }).ToListAsync();
        }

        public async Task<BookListingDetailDTO?> GetListingByIdAsync(int listingId)
        {
            var listing = await _context.BookListings
                .Include(b => b.User)
                .Include(b => b.Category)
                .Where(b => b.BookListingID == listingId)
                .Select(b => new BookListingDetailDTO
                {
                    BookListingID = b.BookListingID,
                    Title = b.Title,
                    Author = b.Author,
                    ISBN = b.ISBN,
                    Edition = b.Edition,
                    Condition = b.Condition,
                    Price = b.Price,
                    Description = b.Description,
                    PhotoURLs = b.PhotoURLs,
                    CategoryName = b.Category != null ? b.Category.Name : null,
                    CategoryID = b.CategoryID,
                    City = b.City,
                    IsSellable = b.IsSellable,
                    IsSwappable = b.IsSwappable,
                    Quantity = b.Quantity,
                    AvailableQuantity = b.AvailableQuantity,
                    AvailabilityStatus = b.AvailabilityStatus,
                    CreatedAt = b.CreatedAt,
                    UserID = b.UserID,
                    OwnerName = b.User.Name,
                    OwnerPhone = b.User.Phone,
                    OwnerEmail = b.User.Email,
                    OwnerRating = b.User.AverageRating,
                    OwnerCity = b.User.City
                })
                .FirstOrDefaultAsync();

            return listing;
        }

        public async Task<IEnumerable<BookListingDisplayDTO>> GetUserListingsAsync(int userId)
        {
            return await _context.BookListings
                .Include(b => b.User)
                .Include(b => b.Category)
                .Where(b => b.UserID == userId)
                .OrderByDescending(b => b.CreatedAt)
                .Select(b => new BookListingDisplayDTO
                {
                    BookListingID = b.BookListingID,
                    Title = b.Title,
                    Author = b.Author,
                    ISBN = b.ISBN,
                    Condition = b.Condition,
                    Price = b.Price,
                    CategoryName = b.Category != null ? b.Category.Name : null,
                    City = b.City,
                    IsSellable = b.IsSellable,
                    IsSwappable = b.IsSwappable,
                    AvailableQuantity = b.AvailableQuantity,
                    AvailabilityStatus = b.AvailabilityStatus,
                    PhotoURLs = b.PhotoURLs,
                    CreatedAt = b.CreatedAt,
                    UserID = b.UserID,
                    OwnerName = b.User.Name,
                    OwnerRating = b.User.AverageRating
                })
                .ToListAsync();
        }

        public async Task<BookListing?> CreateListingAsync(BookListing listing)
        {
            _context.BookListings.Add(listing);
            await _context.SaveChangesAsync();
            return listing;
        }

        public async Task<BookListing?> UpdateListingAsync(BookListing listing)
        {
            _context.BookListings.Update(listing);
            await _context.SaveChangesAsync();
            return listing;
        }

        public async Task<bool> DeleteListingAsync(int listingId)
        {
            var listing = await _context.BookListings.FindAsync(listingId);
            if (listing == null)
                return false;

            _context.BookListings.Remove(listing);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UserOwnsListingAsync(int userId, int listingId)
        {
            return await _context.BookListings
                .AnyAsync(b => b.BookListingID == listingId && b.UserID == userId);
        }

        public async Task<int> GetTotalListingsCountAsync(BookListingFilterDTO? filter = null)
        {
            var query = _context.BookListings
                .Where(b => b.AvailableQuantity > 0)
                .AsQueryable();

            if (filter != null)
            {
                if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
                {
                    var searchLower = filter.SearchTerm.ToLower();
                    query = query.Where(b =>
                        b.Title.ToLower().Contains(searchLower) ||
                        (b.Author != null && b.Author.ToLower().Contains(searchLower)) ||
                        (b.ISBN != null && b.ISBN.Contains(searchLower))
                    );
                }

                if (filter.CategoryID.HasValue)
                    query = query.Where(b => b.CategoryID == filter.CategoryID.Value);

                if (filter.Condition.HasValue)
                    query = query.Where(b => b.Condition == filter.Condition.Value);

                if (filter.MinPrice.HasValue)
                    query = query.Where(b => b.Price >= filter.MinPrice.Value);

                if (filter.MaxPrice.HasValue)
                    query = query.Where(b => b.Price <= filter.MaxPrice.Value);

                if (!string.IsNullOrWhiteSpace(filter.City))
                    query = query.Where(b => b.City == filter.City);

                if (filter.IsSellable.HasValue)
                    query = query.Where(b => b.IsSellable == filter.IsSellable.Value);

                if (filter.IsSwappable.HasValue)
                    query = query.Where(b => b.IsSwappable == filter.IsSwappable.Value);
            }

            return await query.CountAsync();
        }

        public async Task UpdateBookListingAsync(BookListing listing)
        {
            _context.BookListings.Update(listing);
            await _context.SaveChangesAsync();
        }

        public async Task<BookListing?> GetBookListingByIdAsync(int id)
        {
            return await _context.BookListings
                .Include(b => b.User)
                .FirstOrDefaultAsync(b => b.BookListingID == id);
        }
    }
}