using ShelfLife.Models;

namespace ShelfLife.Repository
{
    public class BookRepository : MainRepository<Book>
    {
        public BookRepository(DBcontext context) : base(context)
        {
        }
    }
}
