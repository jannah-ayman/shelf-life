using ShelfLife.Models;

namespace ShelfLife.Repository
{
    public class BookListingRepository : MainRepository<BookListing>
    {
        public BookListingRepository(DBcontext context) : base(context)
        {
        }
    }
}
