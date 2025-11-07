using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShelfLife.Models;

namespace ShelfLife.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class dashboarOrgController : ControllerBase
    {
        DBcontext context;
        public dashboarOrgController(DBcontext _context)
        {
            context = _context;
        }
        [HttpPost]
        public IActionResult CreateBook(Book b)
        {

            context.Books.Add(b);
            context.SaveChanges();
            return Ok();
        }
        [HttpGet]
        public IActionResult getBooks()
        {
            List<Book> books = context.Books.ToList();
            return Ok(books);
        }
    }
}
