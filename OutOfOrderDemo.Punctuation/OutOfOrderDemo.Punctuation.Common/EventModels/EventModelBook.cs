using OutOfOrderDemo.Punctuation.Common;

namespace OutOfOrderDemo.Punctuation.Common;

public class EventModelBook
{
    public Guid EntityId { get; set; }
    public DateTime Published { get; set; }
    public string Title { get; set; }
    public Guid AuthorId { get; set; }
}
