using MassTransit;
using OutOfOrderDemo.Punctuation.Api;
using OutOfOrderDemo.Punctuation.Common;

namespace OutOfOrderDemo.Punctuation.Listener.Consumers;

public class ConsumerAuthorCreated
    : IConsumer<EventAuthorCreated>
{
    private readonly BaseRepository<BookAuthor> _authorsRepository;
    private readonly BaseRepository<BookExpanded> _booksRepository;

    public ConsumerAuthorCreated(
        BaseRepository<BookAuthor> authors, 
        BaseRepository<BookExpanded> booksRepository)
    {
        _authorsRepository = authors;
        _booksRepository = booksRepository;
    }

    public async Task Consume(
        ConsumeContext<EventAuthorCreated> context)
    {
        EventAuthorCreated @event = context.Message;

        // insert the author. 
        // make other checks (like duplicates)
        BookAuthor bookAuthor = new();
        bookAuthor.Consume(@event);

        await _authorsRepository.InsertAsync(bookAuthor);

        // update books if there are any where the author was not present
        // when book was received

        // Note: we can also use the book version to filter books 
        List<BookExpanded> books = await _booksRepository.FindManyAsync(
            w => w.Author.AuthorId == @event.EventModel.EntityId);

        foreach(var book in books)
        {
            book.Consume(bookAuthor);
            await _booksRepository.UpdateAsync(book);
        }
    }
}
