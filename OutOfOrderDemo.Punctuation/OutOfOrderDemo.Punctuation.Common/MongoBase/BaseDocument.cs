using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace OutOfOrderDemo.Punctuation.Common;
public class BaseDocument
{
    [BsonId(IdGenerator = typeof(GuidGenerator))]
    [BsonIgnoreIfDefault]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; }

    [BsonIgnore]
    public bool IsIdValid => Id != Guid.Empty;


    [BsonConstructor]
    public BaseDocument()
    {
    }

    public BaseDocument(Guid id)
    {
        Id = id;
    }
}
