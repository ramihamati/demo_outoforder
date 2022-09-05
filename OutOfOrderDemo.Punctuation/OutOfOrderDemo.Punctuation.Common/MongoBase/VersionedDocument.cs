using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace OutOfOrderDemo.Punctuation.Common;

public class VersionedDocument<TDocument> :
    BaseDocument<TDocument>,
    IEntityWithVersion
    where TDocument : BaseDocument<TDocument>
{
    [BsonElement("EntityVersion")]
    private long _version;

    [BsonIgnore]
    public long EntityVersion => _version;

    public long SetNextVersion()
    {
        _version++;
        return _version;
    }

    [BsonConstructor]
    protected VersionedDocument()
    {
    }
}
