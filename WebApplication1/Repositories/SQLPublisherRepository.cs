using WebApplication1.Data;
using WebApplication1.Models.Domain;
using WebApplication1.Models.DTO;

namespace WebApplication1.Repositories
{
    public class SQLPublisherRepository : IPublisherRepository
    {
        private readonly AppDbContext _dbContext;

        public SQLPublisherRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<PublisherDTO> GetAllPublishers()
        {
            var allPublishersDomain = _dbContext.Publishers.ToList();

            var allPublisherDTO = allPublishersDomain.Select(publisherDomain => new PublisherDTO
            {
                Id = publisherDomain.Id,
                Name = publisherDomain.Name
            }).ToList();

            return allPublisherDTO;
        }

        public PublisherNoIdDTO? GetPublisherById(int id)
        {
            var publisherWithIdDomain = _dbContext.Publishers.FirstOrDefault(x => x.Id == id);

            if (publisherWithIdDomain == null)
            {
                return null;
            }

            return new PublisherNoIdDTO
            {
                Name = publisherWithIdDomain.Name
            };
        }

        public AddPublisherRequestDTO AddPublisher(AddPublisherRequestDTO addPublisherRequestDTO)
        {
            var publisherDomainModel = new Publisher
            {
                Name = addPublisherRequestDTO.Name
            };

            _dbContext.Publishers.Add(publisherDomainModel);
            _dbContext.SaveChanges();

            return addPublisherRequestDTO;
        }

        public PublisherNoIdDTO? UpdatePublisherById(int id, PublisherNoIdDTO publisherNoIdDTO)
        {
            var publisherDomain = _dbContext.Publishers.FirstOrDefault(n => n.Id == id);

            if (publisherDomain == null)
            {
                return null;
            }

            publisherDomain.Name = publisherNoIdDTO.Name;
            _dbContext.SaveChanges();

            return publisherNoIdDTO;
        }

        public Publisher? DeletePublisherById(int id)
        {
            var publisherDomain = _dbContext.Publishers.FirstOrDefault(n => n.Id == id);

            if (publisherDomain != null)
            {
                _dbContext.Publishers.Remove(publisherDomain);
                _dbContext.SaveChanges();
            }

            return publisherDomain;

        }
        public List<BookWithAuthorAndPublisherDTO> GetBooksByPublisherId(int publisherId)
        {
            var books = _dbContext.Books
                .Where(b => b.PublisherID == publisherId)
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