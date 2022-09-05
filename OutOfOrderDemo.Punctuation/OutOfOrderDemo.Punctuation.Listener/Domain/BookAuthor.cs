using OutOfOrderDemo.Punctuation.Common;

namespace OutOfOrderDemo.Punctuation.Api;

public class BookAuthor
    : MultiVersionDocument<BookAuthor>
{
    public BookAuthor() 
        : base("author_")
    {
    }

    public string FirstName { get; set; }
    public string LastName { get; set; }

    public void Consume(EventAuthorCreated model)
    {
        // Id required for version generate

        Id = model.EventModel.EntityId;
        // check the order of the event
        if (!CanSetOwnVersion(model.Version))
        {
            return;
        }
        SetOwnVersion(model.Version);

        FirstName = model.EventModel.FirstName;
        LastName = model.EventModel.LastName;

        RemoveReferenceVersions();
    }

    public void Consume(EventAuthorUpdated model)
    {
        if (this.Id != model.EventModel.EntityId)
        {
            return;
        }

        // check the order of the event
        if (!CanSetOwnVersion(model.Version))
        {
            return;
        }
        SetOwnVersion(model.Version);

        FirstName = model.EventModel.FirstName;
        LastName = model.EventModel.LastName;
    }
}
