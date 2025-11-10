//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using ShelfLife.DTOs;
//using ShelfLife.Models;
//using ShelfLife.Repository;

//namespace ShelfLife.Controllers
//{
    
//    [Route("api/[controller]")]
//    [ApiController]
//    public class dashboarOrgController : ControllerBase
//    {
//        private readonly BookRepository _repo;
//        private readonly RequestRepository _reqRepo;
//        public dashboarOrgController(BookRepository repo , RequestRepository reqRepo)
//        {
//            _repo = repo;
//            _reqRepo = reqRepo;
//        }

//        [HttpGet]
//        public IActionResult GetAllBooks()
//        {
//            var books = _repo.GetAll();
//            return Ok(books);
//        }

        
//        [HttpGet("{id}")]
//        public IActionResult GetBook(int id)
//        {
//            var book = _repo.FindById(id);
//            if (book == null)
//                return NotFound();

//            return Ok(book);
//        }

//        // ✅ 3) Add book
//        [HttpPost]
//        public IActionResult AddBook([FromBody] CreateBookDto dto)
//        {
//            BookListing b = new BookListing()
//            {
//                ISBN = dto.ISBN,
//                Title = dto.Title,
//                Author = dto.Author,
//                CategoryID = dto.CategoryID,
//                Edition = dto.Edition,
//                Description = dto.Description
//            };

//            _repo.Add(b);
//            _repo.Save();

//            return Ok();
//        }

//        [HttpPut("{id}")]
//        public IActionResult UpdateBook(int id, [FromBody] CreateBookDto dto)
//        {
//            var book = _repo.FindById(id);
//            if (book == null)
//                return NotFound();

//            book.ISBN = dto.ISBN;
//            book.Title = dto.Title;
//            book.Author = dto.Author;
//            book.CategoryID = dto.CategoryID; 
//            book.Edition = dto.Edition;
//            book.Description = dto.Description;

//            _repo.Update(book);
//            _repo.Save();

//            return Ok();
//        }

//        [HttpDelete("{id}")]
//        public IActionResult DeleteBook(int id)
//        {
//            var book = _repo.FindById(id);
//            if (book == null)
//                return NotFound();

//            _repo.Delete(book);
//            _repo.Save();

//            return Ok();
//        }

//        [HttpGet("search")]
//        public IActionResult SearchBooks([FromQuery] string title)
//        {
//            if (string.IsNullOrWhiteSpace(title))
//                return BadRequest("Title is required.");

//            // استخدمي LINQ للبحث داخل الـ DbSet
//            var books = _repo.GetAll()
//                             .Where(b => b.Title.Contains(title, StringComparison.OrdinalIgnoreCase))
//                             .ToList();

//            if (!books.Any())
//                return NotFound("No books found with the given title.");

//            return Ok(books);
//        }
//        //------------------------requests----------------------------
//        [HttpGet("requests")]
//        public IActionResult GetAllRequests()
//        {
//            var requests = _reqRepo.GetAll()
//                                       .Select(r => new RequestDto
//                                       {
//                                           OrderID = r.OrderID,
//                                           BookID = r.BookID,
//                                           BookTitle = r.Book.Title,
//                                           RequesterName = r.User.ProfileName,
//                                           Status = r.Status,
//                                           RequestDate = r.CreatedAt
//                                       })
//                                       .ToList();

//            return Ok(requests);
//        }

//        [HttpPost("requests/{id}/approve")]
//        public IActionResult ApproveRequest(int id)
//        {
//            var request = _reqRepo.FindById(id);
//            if (request == null) return NotFound();

//            request.Status = "Approved";
//            _reqRepo.Update(request);
//            _reqRepo.Save();

//            return Ok(request);
//        }

//        [HttpPost("requests/{id}/reject")]
//        public IActionResult RejectRequest(int id)
//        {
//            var request = _reqRepo.FindById(id);
//            if (request == null) return NotFound();

//            request.Status = "Rejected";
//            _reqRepo.Update(request);
//            _reqRepo.Save();

//            return Ok(request);
//        }

//    }
//}
