namespace OutOfOrderDemo.Punctuation.Common;

public interface IMultiVersionDocument
{
    long GetVersion(VersionKey key);
    void SetVersion(VersionKey key, long value);
    bool CanSetVersion(VersionKey key, long value);
    void RemoveVersions(List<VersionKey> keys);
}
