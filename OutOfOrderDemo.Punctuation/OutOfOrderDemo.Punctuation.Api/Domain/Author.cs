using OutOfOrderDemo.Punctuation.Common;

namespace OutOfOrderDemo.Punctuation.Api;

public class Author
    : VersionedDocument<Author>
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
}
