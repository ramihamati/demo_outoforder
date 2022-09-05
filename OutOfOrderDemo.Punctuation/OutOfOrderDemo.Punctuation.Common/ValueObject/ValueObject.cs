using System.Runtime.Serialization;

namespace OutOfOrderDemo.Punctuation.Common;

[Serializable]
public abstract class ValueObject<TValue> : IComparable<ValueObject<TValue>>, IEquatable<ValueObject<TValue>>, ISerializable
{
    public TValue Value { get; protected set; }

    protected Type Type => typeof(TValue);

    protected abstract bool EqualsCore(object obj);

    protected abstract int GetHashCodeCore();

    public abstract int CompareTo(ValueObject<TValue> other);

    public bool Equals(ValueObject<TValue> other)
    {
        return EqualsCore(other);
    }

    public override bool Equals(object obj)
    {
        return EqualsCore(obj);
    }

    public override int GetHashCode()
    {
        return GetHashCodeCore();
    }

    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        Value = (TValue)info.GetValue("Value", Type);
    }

    public static bool operator ==(ValueObject<TValue> first, ValueObject<TValue> second)
    {
        if ((object)first == null && (object)second == null)
        {
            return true;
        }

        if ((object)first == null && (object)second != null)
        {
            return false;
        }

        if ((object)first != null && (object)second == null)
        {
            return false;
        }

        return first.Equals(second);
    }

    public static bool operator !=(ValueObject<TValue> first, ValueObject<TValue> second)
    {
        return !(first == second);
    }

    public static bool operator >(ValueObject<TValue> first, ValueObject<TValue> second)
    {
        if ((object)first == null && (object)second == null)
        {
            return false;
        }

        if ((object)first == null && (object)second != null)
        {
            return false;
        }

        if ((object)first != null && (object)second == null)
        {
            return true;
        }

        return first.CompareTo(second) > 0;
    }

    public static bool operator <(ValueObject<TValue> first, ValueObject<TValue> second)
    {
        if ((object)first == null && (object)second == null)
        {
            return false;
        }

        if ((object)first == null && (object)second != null)
        {
            return true;
        }

        if ((object)first != null && (object)second == null)
        {
            return false;
        }

        return first.CompareTo(second) < 0;
    }

    public static bool operator >=(ValueObject<TValue> first, ValueObject<TValue> second)
    {
        if ((object)first == null && (object)second == null)
        {
            return true;
        }

        if ((object)first == null && (object)second != null)
        {
            return false;
        }

        if ((object)first != null && (object)second == null)
        {
            return true;
        }

        return first.CompareTo(second) >= 0;
    }

    public static bool operator <=(ValueObject<TValue> first, ValueObject<TValue> second)
    {
        if ((object)first == null && (object)second == null)
        {
            return true;
        }

        if ((object)first == null && (object)second != null)
        {
            return true;
        }

        if ((object)first != null && (object)second == null)
        {
            return false;
        }

        return first.CompareTo(second) <= 0;
    }

    public static implicit operator TValue(ValueObject<TValue> valueObject)
    {
        return ((object)valueObject != null) ? valueObject.Value : default(TValue);
    }
}
