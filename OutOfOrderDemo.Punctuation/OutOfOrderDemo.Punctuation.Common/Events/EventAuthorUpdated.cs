namespace OutOfOrderDemo.Punctuation.Common;

public class EventAuthorUpdated 
{
    public long Version { get; set; }

    // Updated model
    public EventModelAuthor EventModel { get; set; }
}
