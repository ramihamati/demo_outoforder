using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using System.Reflection;

namespace OutOfOrderDemo.Punctuation.Common;

public abstract class ValueObjectSerializer<T> : IBsonSerializer<T>, IBsonSerializer where T : class
{
    private static readonly string _valuePropertyName;

    private static readonly Dictionary<Type, ConstructorInfo> _cachedConstructorInfos;

    private static readonly Dictionary<Type, PropertyInfo> _cachedPropertyInfos;

    public Type ValueType { get; }

    protected Func<object, T> _factory { get; }

    static ValueObjectSerializer()
    {
        _cachedConstructorInfos = new Dictionary<Type, ConstructorInfo>();
        _cachedPropertyInfos = new Dictionary<Type, PropertyInfo>();
        _valuePropertyName = "Value";
    }

    protected ValueObjectSerializer()
    {
        ValueType = typeof(T);
        ConstructorInfo cinfo = GetConstructorInfo(ValueType);
        PropertyInfo propertyInfo = GetPropertyInfo(ValueType);
        _factory = delegate (object value)
        {
            T val = cinfo.Invoke(null) as T;
            if (val == null)
            {
                throw new Exception("Could not create an instance of type " + ValueType.Name);
            }

            propertyInfo.SetValue(val, value);
            return val;
        };
    }

    private PropertyInfo GetPropertyInfo(Type t)
    {
        if (_cachedPropertyInfos.ContainsKey(t))
        {
            return _cachedPropertyInfos[t];
        }

        PropertyInfo property = ValueType.GetProperty(_valuePropertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (property == null)
        {
            throw new Exception("Could not get the property " + _valuePropertyName + " from type " + ValueType.Name);
        }

        if (!property.CanWrite)
        {
            throw new Exception("Cannot write to property " + _valuePropertyName + " of type " + ValueType.Name);
        }

        _cachedPropertyInfos.Add(t, property);
        return property;
    }

    private ConstructorInfo GetConstructorInfo(Type t)
    {
        if (_cachedConstructorInfos.ContainsKey(t))
        {
            return _cachedConstructorInfos[t];
        }

        ConstructorInfo constructor = ValueType.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[0], null);
        if (constructor == null)
        {
            throw new Exception("Could not find a parameterless constructor for the ValueObject " + ValueType.Name);
        }

        _cachedConstructorInfos.Add(t, constructor);
        return constructor;
    }

    public abstract T Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args);

    public abstract void Serialize(BsonSerializationContext context, BsonSerializationArgs args, T value);

    object IBsonSerializer.Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        if (context.Reader.CurrentBsonType == BsonType.Null)
        {
            try
            {
                context.Reader.ReadNull();
                ConstructorInfo constructorInfo = GetConstructorInfo(typeof(T));
                return constructorInfo.Invoke(null) as T;
            }
            catch (Exception)
            {
                return null;
            }
        }

        return Deserialize(context, args);
    }

    public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, object value)
    {
        if (value == null)
        {
            context.Writer.WriteNull();
            return;
        }

        T val = value as T;
        if (val != null)
        {
            Serialize(context, args, val);
            return;
        }

        throw new NotSupportedException("Invalid object type for serialization. Expected " + typeof(T).Name + ", received " + value.GetType().Name);
    }
}
