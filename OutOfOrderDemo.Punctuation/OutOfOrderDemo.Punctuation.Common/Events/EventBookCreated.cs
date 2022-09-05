namespace OutOfOrderDemo.Punctuation.Common;

public class EventBookCreated
{
    public long Version { get; set; }
    public EventModelBook EventModel { get; set; }
}