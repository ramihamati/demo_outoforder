using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace OutOfOrderDemo.Punctuation.Common;

[BsonSerializer(typeof(ValueObjectStringSerializer<VersionKey>))]
public class VersionKey : ValueObject<string>
{
    public override int CompareTo(ValueObject<string> other)
    {
        return Value.CompareTo(other.Value);
    }

    protected override bool EqualsCore(object obj)
    {
        if (obj is VersionKey vp)
        {
            return Value.Equals(vp.Value);
        }

        return false;
    }

    protected override int GetHashCodeCore()
    {
        return Value.GetHashCode();
    }

    public static implicit operator VersionKey(string value)
    {
        return new VersionKey() { Value = value };
    }
}
