using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;
using System.Linq.Expressions;

namespace OutOfOrderDemo.Punctuation.Common;
public class MultiVersionDocument<T> :
    BaseDocument<T>,
    IMultiVersionDocument
    where T : BaseDocument<T>
{
    private readonly VersionPrefix _ownVersionPrefix;

    // [BsonIgnore]IReadOnlyDictionary pointing to private Dictionary with [BsonElement] will not work.
    // QUery ContainsKey() fails

    [BsonRequired]
    [BsonElement("Versions")]
    [BsonDictionaryOptions(Representation = DictionaryRepresentation.Document)]
    public Dictionary<string, long> Versions { get; private set; } = new();

    protected MultiVersionDocument(VersionPrefix ownVersionPrefix)
    {
        _ownVersionPrefix = ownVersionPrefix;
    }

    #region [ Versionable fields ]

    protected MultiVersionField<TField> CreateField<TField>(
        string keyPrefix,
        Expression<Func<TField, Guid>> idCollector,
        TField initialValue = null)
        where TField : class
    {
        var field = new MultiVersionField<TField>(
            this,
            keyPrefix,
            idCollector);

        if (initialValue is not null)
        {
            field.SetValue(initialValue);
        }

        return field;
    }

    #endregion

    #region [ Version methods ]

    public long GetVersion(VersionKey key)
    {
        return Versions.ContainsKey(key) ? Versions[key] : -1;
    }

    public bool CanSetVersion(VersionKey key, long value)
    {
        if (Versions.ContainsKey(key))
        {
            return value > Versions[key];
        }
        else
        {
            return true;
        }
    }
    public void SetVersion(VersionKey key, long value)
    {
        if (Versions.ContainsKey(key))
        {
            if (Versions[key] > value)
            {
                throw new Exception($"For ${this.GetType().Name} with {Id} you are trying to set a version \'{key}\' with a value {value}, but the existing version is higher : {Versions[key]}");
            }

            Versions[key] = value;
        }
        else
        {
            Versions.Add(key, value);
        }
    }

    public long GetOwnVersion()
    {
        if (!Versions.ContainsKey(GetOwnVersionKey()))
        {
            throw new ApplicationException($"The ${this.GetType().Name} id {Id} does not have it's main entity state version");
        }

        return Versions[GetOwnVersionKey()];
    }
    public bool CanSetOwnVersion(long value)
    {
        return CanSetVersion(GetOwnVersionKey(), value);
    }
    public void SetOwnVersion(long value)
    {
        SetVersion(GetOwnVersionKey(), value);
    }

    public VersionKey GetOwnVersionKey()
    {  //TODO: create helper for all verision key related operations 
        return VersionKeyBuilderHelper.CreateVersionKey(_ownVersionPrefix, Id);
    }

    public void RemoveVersions(List<VersionKey> keys)
    {
        keys.ForEach(k => Versions.Remove(k));
    }

    /// <summary>
    /// removes all versions except own version
    /// </summary>
    public void RemoveReferenceVersions()
    {
        var ownVersion = GetOwnVersion();
        Versions.Clear();
        Versions.Add(GetOwnVersionKey(), ownVersion);
    }

    #endregion
}
