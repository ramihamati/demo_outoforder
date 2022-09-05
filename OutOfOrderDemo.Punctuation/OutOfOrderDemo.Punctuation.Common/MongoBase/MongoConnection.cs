using MongoDB.Driver;

namespace OutOfOrderDemo.Punctuation.Common;

public class MongoConnection
{
    public MongoClient MongoClient { get; set; }

    public MongoConnection(
        string connectionString)
    {
        MongoClient = new MongoClient(connectionString);
    }

    public IMongoCollection<TDocument> GetCollection<TDocument>(string collectionName, string dbName)
    {
        IMongoDatabase database = this.GetDatabase(dbName);

        return database.GetCollection<TDocument>(collectionName,
          new MongoCollectionSettings
          {
              WriteConcern = WriteConcern.WMajority,
              ReadConcern = ReadConcern.Majority,
              ReadPreference = ReadPreference.PrimaryPreferred
          });
    }

    public IMongoDatabase GetDatabase(string dbName)
    {
        return this.MongoClient.GetDatabase(dbName, new MongoDatabaseSettings
        {
            WriteConcern = WriteConcern.Acknowledged,
            ReadConcern = ReadConcern.Majority,
            ReadPreference = ReadPreference.PrimaryPreferred,
        });
    }

    public void DropDatabase(string dbName, CancellationToken token = default)
    {
        this.MongoClient.DropDatabase(dbName, token);
    }

    public void Dispose()
    {
        this.MongoClient?.Cluster?.Dispose();
    }
}