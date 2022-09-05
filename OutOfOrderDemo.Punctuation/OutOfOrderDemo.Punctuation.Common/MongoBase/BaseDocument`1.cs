using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;

namespace OutOfOrderDemo.Punctuation.Common;

public class BaseDocument<TEntity> : BaseDocument where TEntity : BaseDocument<TEntity>
{
 
}
