namespace OutOfOrderDemo.Punctuation.Common;

public interface IEntityWithVersion
{
    public long EntityVersion { get; }
    public long SetNextVersion();
}
