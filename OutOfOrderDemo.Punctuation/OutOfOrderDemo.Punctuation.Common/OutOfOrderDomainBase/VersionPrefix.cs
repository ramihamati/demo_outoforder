using MongoDB.Bson.Serialization.Attributes;

namespace OutOfOrderDemo.Punctuation.Common;
/**
 * NOTE: We are not mapping the derived entities manually by defining 
 * what is the constructor to be used when deserializing. Perhaps this is the reason
 * why mongo driver will try to create also the base classes. If they are abstract 
 * this will fail. Do not make this class abstract
 */

[BsonSerializer(typeof(ValueObjectStringSerializer<VersionPrefix>))]
public class VersionPrefix : ValueObject<string>
{
    public override int CompareTo(ValueObject<string> other)
    {
        return Value.CompareTo(other.Value);
    }

    protected override bool EqualsCore(object obj)
    {
        if (obj is VersionPrefix vp)
        {
            return Value.Equals(vp.Value);
        }

        return false;
    }

    protected override int GetHashCodeCore()
    {
        return Value.GetHashCode();
    }

    public static implicit operator VersionPrefix(string value)
    {
        return new VersionPrefix() { Value = value};
    }
}
