using MassTransit;
using OutOfOrderDemo.Punctuation.Api;
using OutOfOrderDemo.Punctuation.Common;

namespace OutOfOrderDemo.Punctuation.Listener.Consumers;

public class ConsumerBookUpdated
    : IConsumer<EventBookUpdated>
{
    private readonly BaseRepository<BookAuthor> _authorsRepository;
    private readonly BaseRepository<BookExpanded> _booksRepository;

    public ConsumerBookUpdated(
        BaseRepository<BookAuthor> authors,
        BaseRepository<BookExpanded> booksRepository)
    {
        _authorsRepository = authors;
        _booksRepository = booksRepository;
    }

    public async Task Consume(ConsumeContext<EventBookUpdated> context)
    {
        EventBookUpdated @event = context.Message;

        var book = await _booksRepository.FindAsync(@event.EventModel.EntityId);

        book.Consume(@event);

        await _booksRepository.UpdateAsync(book);
    }
}