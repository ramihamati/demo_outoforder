namespace OutOfOrderDemo.Punctuation.Common;

public class EventBookUpdated 
{
    public long Version { get; set; }

    // Updated model
    public EventModelBook EventModel { get; set; }
}
