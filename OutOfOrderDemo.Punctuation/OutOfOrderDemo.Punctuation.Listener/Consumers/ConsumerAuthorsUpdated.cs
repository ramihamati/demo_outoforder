using MassTransit;
using OutOfOrderDemo.Punctuation.Api;
using OutOfOrderDemo.Punctuation.Common;

namespace OutOfOrderDemo.Punctuation.Listener.Consumers;

public class ConsumerAuthorsUpdated
    : IConsumer<EventAuthorUpdated>
{
    private readonly BaseRepository<BookAuthor> _authorsRepository;
    private readonly BaseRepository<BookExpanded> _booksRepository;

    public ConsumerAuthorsUpdated(
        BaseRepository<BookAuthor> authors,
        BaseRepository<BookExpanded> booksRepository)
    {
        _authorsRepository = authors;
        _booksRepository = booksRepository;
    }

    public async Task Consume(ConsumeContext<EventAuthorUpdated> context)
    {
        EventAuthorUpdated @event = context.Message;

        BookAuthor bookAuthor 
            = await _authorsRepository.FindAsync(@event.EventModel.EntityId);

        bookAuthor.Consume(@event);

        await _authorsRepository.UpdateAsync(bookAuthor);

        // update books if there are any where the author was not present
        // when book was received

        // Note: we can also use the book version to filter books 
        List<BookExpanded> books = await _booksRepository.FindManyAsync(
            w => w.Author.AuthorId == @event.EventModel.EntityId);

        foreach (var book in books)
        {
            book.Consume(bookAuthor);
            await _booksRepository.UpdateAsync(book);
        }
    }
}
