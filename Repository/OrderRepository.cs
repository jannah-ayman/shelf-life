using ShelfLife.Models;

namespace ShelfLife.Repository
{
    public class OrderRepository : MainRepository<Order>
    {
        OrderRepository(DBcontext context) : base(context)
        { }
    }
}
