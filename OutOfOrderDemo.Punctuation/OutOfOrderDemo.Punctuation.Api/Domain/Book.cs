using OutOfOrderDemo.Punctuation.Common;

namespace OutOfOrderDemo.Punctuation.Api;

public class Book
    : VersionedDocument<Book>
{
    public DateTime Published { get; set; }
    public string Title { get; set; }
    public Guid AuthorId { get; set; }
}
