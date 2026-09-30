using WebApplication1.Data;
using WebApplication1.Models.Domain;
using WebApplication1.Models.DTO;

namespace WebApplication1.Repositories
{
    public class SQLAuthorRepository : IAuthorRepository
    {
        private readonly AppDbContext _dbContext;

        public SQLAuthorRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<AuthorDTO> GetAllAuthors()
        {
            var allAuthorsDomain = _dbContext.Authors.ToList();

            var allAuthorDTO = allAuthorsDomain.Select(authorDomain => new AuthorDTO
            {
                Id = authorDomain.Id,
                FullName = authorDomain.FullName
            }).ToList();

            return allAuthorDTO;
        }

        public AuthorNoIdDTO? GetAuthorById(int id)
        {
            var authorWithIdDomain = _dbContext.Authors.FirstOrDefault(x => x.Id == id);

            if (authorWithIdDomain == null)
            {
                return null;
            }

            return new AuthorNoIdDTO
            {
                FullName = authorWithIdDomain.FullName
            };
        }

        public AddAuthorRequestDTO AddAuthor(AddAuthorRequestDTO addAuthorRequestDTO)
        {
            var authorDomainModel = new Author
            {
                FullName = addAuthorRequestDTO.FullName
            };

            _dbContext.Authors.Add(authorDomainModel);
            _dbContext.SaveChanges();

            return addAuthorRequestDTO;
        }

        public AuthorNoIdDTO? UpdateAuthorById(int id, AuthorNoIdDTO authorNoIdDTO)
        {
            var authorDomain = _dbContext.Authors.FirstOrDefault(n => n.Id == id);

            if (authorDomain == null)
            {
                return null;
            }

            authorDomain.FullName = authorNoIdDTO.FullName;
            _dbContext.SaveChanges();

            return authorNoIdDTO;
        }

        public Author? DeleteAuthorById(int id)
        {
            var authorDomain = _dbContext.Authors.FirstOrDefault(n => n.Id == id);

            if (authorDomain != null)
            {
                _dbContext.Authors.Remove(authorDomain);
                _dbContext.SaveChanges();
            }

            return authorDomain;
        }
        public List<BookWithAuthorAndPublisherDTO> GetBooksByAuthorId(int authorId)
        {
            var books = _dbContext.Books
                .Where(b => b.Book_Authors.Any(ba => ba.AuthorId == authorId))
                .Select(book => new BookWithAuthorAndPublisherDTO
                {
                    Id = book.Id,
                    Title = book.Title,
                    Description = book.Description,
                    IsRead = book.IsRead,
                    DateRead = book.IsRead ? book.DateRead : null,
                    Rate = book.IsRead ? book.Rate : null,
                    Genre = book.Genre,
                    CoverUrl = book.CoverUrl,
                    PublisherName = book.Publisher.Name,
                    AuthorNames = book.Book_Authors.Select(ba => ba.Author.FullName).ToList()
                })
                .ToList();

            return books;
        }
    }
}