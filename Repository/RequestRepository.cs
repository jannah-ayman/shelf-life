using ShelfLife.Models;

namespace ShelfLife.Repository
{
    public class RequestRepository : MainRepository<Request>
    {
        RequestRepository(DBcontext context) : base(context)
        { }
    }
}
