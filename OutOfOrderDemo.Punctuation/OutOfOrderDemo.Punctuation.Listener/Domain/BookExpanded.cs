using MongoDB.Bson.Serialization.Attributes;
using OutOfOrderDemo.Punctuation.Common;

namespace OutOfOrderDemo.Punctuation.Api;

public record BookExpandedAuthor
{
    public Guid AuthorId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
}

public class BookExpanded
    : MultiVersionDocument<BookExpanded>
{
  
    // complex object taking in account version of the document
    private readonly MultiVersionField<BookExpandedAuthor> _author;

    [BsonElement("Author")]
    [BsonRequired]
    public BookExpandedAuthor Author
    {
        get => _author.GetValue()!;
        set => _author.SetValue(value);
    }
    public DateTime Published { get; set; }
    public string Title { get; set; }
    
    public BookExpanded() 
        : base("book_")
    {
        _author = CreateField<BookExpandedAuthor>(
            keyPrefix: "author_",
            idCollector: r => r.AuthorId,
            initialValue: new BookExpandedAuthor());
    }

    public void Consume(EventBookCreated model)
    {
        Id = model.EventModel.EntityId;

        // check the order of the event
        if (!CanSetOwnVersion(model.Version))
        {
            return;
        }
        SetOwnVersion(model.Version);

        Title = model.EventModel.Title;
        Published = model.EventModel.Published;

        RemoveReferenceVersions();
    }

    public void Consume(EventBookUpdated model)
    {
        Id = model.EventModel.EntityId;

        // check the order of the event
        if (!CanSetOwnVersion(model.Version))
        {
            return;
        }
        SetOwnVersion(model.Version);

        Title = model.EventModel.Title;
        Published = model.EventModel.Published;
        
        _author.SetValue(new BookExpandedAuthor
        {
            AuthorId = model.EventModel.AuthorId
        });
    }

    public bool Consume(BookAuthor model)
    {
        if (model.Id != this.Author.AuthorId)
        {
            return false;
        }

        return _author.SetValue(Author with
        {
            LastName = model.LastName,
            FirstName = model.LastName
        }, model.GetOwnVersion());
    }
}
