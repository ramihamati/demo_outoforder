using MongoDB.Bson;
using MongoDB.Bson.Serialization;

namespace OutOfOrderDemo.Punctuation.Common;

public class ValueObjectStringSerializer<T> : ValueObjectSerializer<T> where T : ValueObject<string>
{
    public override T Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        if (context.Reader.CurrentBsonType == BsonType.Null)
        {
            context.Reader.ReadNull();
            return null;
        }

        string arg = context.Reader.ReadString();
        return base._factory(arg);
    }

    public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, T value)
    {
        if (value != null)
        {
            if (value.Value == null)
            {
                context.Writer.WriteNull();
            }
            else
            {
                context.Writer.WriteString(value.Value);
            }
        }
    }
}
