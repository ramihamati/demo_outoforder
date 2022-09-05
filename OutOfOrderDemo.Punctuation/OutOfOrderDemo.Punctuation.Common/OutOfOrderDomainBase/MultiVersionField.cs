using System.Linq.Expressions;

namespace OutOfOrderDemo.Punctuation.Common;


// the item is versionable
public class MultiVersionField<TField> 
    where TField : class
{
    private readonly IMultiVersionDocument _owner;
    private readonly string _keyPrefix;
    private readonly Expression<Func<TField, Guid>> _idCollector;
    private TField? field;

    public MultiVersionField(
        IMultiVersionDocument owner,
        string keyPrefix,
        Expression<Func<TField, Guid>> idCollector)
    {
        _owner = owner;
        _keyPrefix = keyPrefix;
        _idCollector = idCollector;
    }

    public TField? GetValue() => field;

    /// <summary>
    /// Sets (update/replace) a field and it's version accordingly
    /// </summary>
    public bool SetValue(TField value, long? version = null)
    {
        // if version is present - the update is from the event having that version.
        //            - e.g. for QActivity.ActivityType -> event IEventActivityTypeUpdated
        // if version is not present
        //          - the field is from a different model
        //          - e.g. QActivity.ActivityTypeId received in IEventActivity
        //          - for this event -> we remove the old version if the key is different only
        //          -    e.g. if activityTypeId is different, we remove version. otherwise, we keep it. We will update on newer versions
        Guid newId = _idCollector.Compile()(value);
        VersionKey newKey = VersionKeyBuilderHelper.CreateVersionKey(_keyPrefix, newId);
        if (version.HasValue)
        {
            if (field is null)
            {
                // this is the first set
                field = value;
                _owner.SetVersion(newKey, version.Value);
                return true;
            }
            else
            {
                Guid oldId = _idCollector.Compile()(field);
                VersionKey oldKey = VersionKeyBuilderHelper.CreateVersionKey(_keyPrefix, oldId);

                if (oldId != newId)
                {
                    _owner.RemoveVersions(new List<VersionKey> { oldKey });
                }

                // check if the version is newer
                long existingVersion = _owner.GetVersion(oldKey);
                if (existingVersion < 0)
                {
                    // we don't have the item or it was removed
                    field = value;
                    _owner.SetVersion(newKey, version.Value);
                    return true;
                }
                else
                {
                    if (version.Value > existingVersion)
                    {
                        field = value;
                        _owner.SetVersion(newKey, version.Value);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }
        else
        {
            // new version does not exist. But we must check if there is an older version but for a different 
            // entity id. In that case we should remove id

            if (field is not null)
            {
                Guid oldId = _idCollector.Compile()(field);
                VersionKey oldKey = VersionKeyBuilderHelper.CreateVersionKey(_keyPrefix, oldId);

                if (oldId != newId)
                {
                    _owner.RemoveVersions(new List<VersionKey> { oldKey });
                }
            }
            // no version is passed. We just write the field. 
            // if there is a version set in the dictionary, we leave it. It's closest to a final version then starting from 0
            field = value;
            return true;
        }
    }
}
