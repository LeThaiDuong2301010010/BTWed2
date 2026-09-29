using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models.Domain;
using WebApplication1.Models.DTO;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        public BooksController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("get-all-books")]
        public IActionResult GetAll()
        {
            var allBooksDomain = _dbContext.Books
                .Include(b => b.Publisher)
                .Include(b => b.Book_Authors)
                    .ThenInclude(ba => ba.Author);

            var allBooksDTO = allBooksDomain.Select(book => new BookWithAuthorAndPublisherDTO
            {
                Id = book.Id,
                Title = book.Title,
                Description = book.Description,
                IsRead = book.IsRead,
                DateRead = book.IsRead ? book.DateRead : null,
                Rate = book.IsRead ? book.Rate : null,
                Genre = book.Genre,
                CoverUrl = book.CoverUrl,
                DateAdded = book.DateAdded,
                PublisherName = book.Publisher != null ? book.Publisher.Name : "Unknown",
                AuthorNames = book.Book_Authors != null
                    ? book.Book_Authors
                        .Where(n => n.Author != null)
                        .Select(n => n.Author.FullName)
                        .ToList()
                    : new List<string>()
            }).ToList();

            return Ok(allBooksDTO);
        }
        [HttpPost("add-book")]
        public IActionResult AddBook([FromBody] AddBookRequestDTO addBookRequestDTO)
        {
            // Check if publisher exists
            var publisherDomain = _dbContext.Publishers
                .FirstOrDefault(x => x.Id == addBookRequestDTO.PublisherID);

            if (publisherDomain == null)
            {
                return NotFound(new { message = "Không tìm thấy NXB" });
            }

            // Create a new book domain object
            var bookDomain = new Book
            {
                Title = addBookRequestDTO.Title ?? string.Empty,
                Description = addBookRequestDTO.Description ?? string.Empty,
                IsRead = addBookRequestDTO.IsRead,
                DateRead = addBookRequestDTO.DateRead,
                Rate = addBookRequestDTO.Rate,
                Genre = addBookRequestDTO.Genre ?? string.Empty,
                CoverUrl = addBookRequestDTO.CoverUrl,
                DateAdded = addBookRequestDTO.DateAdded == default ? DateTime.Now : addBookRequestDTO.DateAdded,
                PublisherID = publisherDomain.Id
            };

            _dbContext.Books.Add(bookDomain);
            _dbContext.SaveChanges();

            // Add authors
            if (addBookRequestDTO.AuthorIds != null)
            {
                foreach (var authorId in addBookRequestDTO.AuthorIds)
                {
                    var authorDomain = _dbContext.Authors.FirstOrDefault(x => x.Id == authorId);
                    if (authorDomain == null)
                    {
                        return NotFound(new { message = $"Không tìm thấy tác giả Id = {authorId}" });
                    }

                    var bookAuthorDomain = new Book_Author
                    {
                        BookId = bookDomain.Id,
                        AuthorId = authorDomain.Id
                    };
                    _dbContext.Books_Authors.Add(bookAuthorDomain);
                }
                _dbContext.SaveChanges();
            }

            return Ok(new { message = "Thêm sách thành công", bookId = bookDomain.Id });
        }
        [HttpDelete("delete-book-by-id/{id:int}")]
        public IActionResult DeleteBookById(int id)
        {
            var bookDomain = _dbContext.Books.FirstOrDefault(x => x.Id == id);
            if (bookDomain == null)
            {
                return NotFound(new { message = "Không tìm thấy sách để xóa" });
            }

            // Xóa Book_Author liên quan trước
            var existingBookAuthors = _dbContext.Books_Authors
                .Where(x => x.BookId == id).ToList();

            if (existingBookAuthors != null && existingBookAuthors.Count > 0)
            {
                _dbContext.Books_Authors.RemoveRange(existingBookAuthors);
                _dbContext.SaveChanges();
            }

            _dbContext.Books.Remove(bookDomain);
            _dbContext.SaveChanges();

            return Ok(new { message = "Xóa sách thành công" });
        }
    }
}