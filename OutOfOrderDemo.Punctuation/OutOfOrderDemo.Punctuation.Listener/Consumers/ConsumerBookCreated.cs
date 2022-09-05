using MassTransit;
using OutOfOrderDemo.Punctuation.Api;
using OutOfOrderDemo.Punctuation.Common;

namespace OutOfOrderDemo.Punctuation.Listener.Consumers;

public class ConsumerBookCreated
    : IConsumer<EventBookCreated>
{
    private readonly BaseRepository<BookAuthor> _authorsRepository;
    private readonly BaseRepository<BookExpanded> _booksRepository;

    public ConsumerBookCreated(
        BaseRepository<BookAuthor> authors,
        BaseRepository<BookExpanded> booksRepository)
    {
        _authorsRepository = authors;
        _booksRepository = booksRepository;
    }

    public async Task Consume(ConsumeContext<EventBookCreated> context)
    {
        EventBookCreated @event = context.Message;

        var author = await _authorsRepository.FindAsync(
            @event.EventModel.AuthorId);

        var book = new BookExpanded();
        
        book.Consume(@event);

        if (author is not null)
        {
            book.Consume(author);
        }

        await _booksRepository.InsertAsync(book);
    }
}
