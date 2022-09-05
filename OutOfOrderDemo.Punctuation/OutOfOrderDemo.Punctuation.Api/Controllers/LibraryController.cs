using MassTransit;
using Microsoft.AspNetCore.Mvc;
using OutOfOrderDemo.Punctuation.Common;

namespace OutOfOrderDemo.Punctuation.Api.Controllers
{
    [ApiController]
    [Route("library")]
    public class LibraryController : ControllerBase
    {
        private readonly BaseRepository<Author> _authorRepository;
        private readonly BaseRepository<Book> _bookRepository;
        private readonly IBusControl _busControl;

        public LibraryController(
            BaseRepository<Author> authorRepository,
            BaseRepository<Book> bookRepository,
            IBusControl busControl)
        {
            _authorRepository = authorRepository;
            _bookRepository = bookRepository;
            _busControl = busControl;
        }

        [HttpPost("author")]
        public async Task<IActionResult> CreateAuthorAsync(
            [FromForm] string firstName,
            [FromForm] string lastName)
        {
            Author author = new()
            {
                FirstName = firstName,
                LastName = lastName
            };

            await _authorRepository.InsertAsync(author);

            await _busControl.Publish(new EventAuthorCreated
            {
                EventModel = new EventModelAuthor
                {
                    EntityId = author.Id,
                    FirstName = author.FirstName,
                    LastName = author.LastName
                },
                Version = author.EntityVersion
            });

            // this is just a demo. You shouldn't return the domain object
            return Ok(author);
        }

        [HttpPut("author")]
        public async Task<IActionResult> UpdateAuthorAsync(
            [FromForm] Guid id,
            [FromForm] string firstName,
            [FromForm] string lastName)
        {
            Author author = await _authorRepository.FindAsync(id);
            author.FirstName = firstName;
            author.LastName = lastName;

            // the version will be increased
            await _authorRepository.UpdateAsync(author);

            await _busControl.Publish(new EventAuthorUpdated
            {
                EventModel = new EventModelAuthor
                {
                    EntityId = author.Id,
                    FirstName = author.FirstName,
                    LastName = author.LastName
                },
                Version = author.EntityVersion
            });

            return Accepted(author);
        }

        [HttpPost("book")]
        public async Task<IActionResult> PublishBook(
            [FromForm] string title,
            [FromForm] Guid authorId)
        {
            Author author = await _authorRepository.FindAsync(authorId);
            // validate that author exists

            var book = new Book
            {
                Title = title,
                Published = DateTime.UtcNow,
                AuthorId = author.Id
            };

            await _bookRepository.InsertAsync(book);

            await _busControl.Publish(new EventBookCreated
            {
                Version = book.EntityVersion,
                EventModel = new EventModelBook
                {
                    AuthorId = authorId,
                    EntityId = book.Id,
                    Published = book.Published,
                    Title = book.Title
                }
            });

            return Ok(book);
        }

        [HttpPut("book")]
        public async Task<IActionResult> UpdateBook(
            [FromForm] string title,
            [FromForm] Guid bookId)
        {
            Book book = await _bookRepository.FindAsync(bookId);

            // validate that book exists

            book.Title = title;

            // book version will be increased
            await _bookRepository.UpdateAsync(book);

            await _busControl.Publish(new EventBookUpdated
            {
                Version = book.EntityVersion,
                EventModel = new EventModelBook
                {
                    AuthorId = book.AuthorId,
                    EntityId = book.Id,
                    Published = book.Published,
                    Title = book.Title
                }
            });

            return Accepted();
        }
    }
}
