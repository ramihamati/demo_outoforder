using MongoDB.Driver;
using System.Linq.Expressions;

namespace OutOfOrderDemo.Punctuation.Common;

public class BaseRepository<TEntity>
    where TEntity : BaseDocument<TEntity>
{
    private readonly IMongoCollection<TEntity> _collection;

    public BaseRepository(IMongoCollection<TEntity> collection)
    {
        this._collection = collection;
    }

    public Task DeleteAsync(List<TEntity> entities)
    {

        if (entities.Count > 0)
        {
            foreach (TEntity entity in entities)
            {
                if (entity is IEntityWithVersion ewv)
                {
                    ewv.SetNextVersion();
                }
            }

            return _collection.DeleteManyAsync(
                r => entities.ConvertAll(t => t.Id).Contains(r.Id));
        }
        return Task.CompletedTask;
    }

    public Task DeleteAsync(TEntity entity)
    {
        if (entity is IEntityWithVersion ewv)
        {
            ewv.SetNextVersion();
        }

        return _collection.DeleteOneAsync(r => r.Id == entity.Id);
    }

    public async Task<TEntity> FindAsync(Guid entityId)
    {
        return (await _collection.FindAsync(
            filter: r => r.Id == entityId)).SingleOrDefault();
    }

    public async Task<List<TEntity>> FindManyAsync(
        Expression<Func<TEntity, bool>> query)
    {
        FilterDefinition<TEntity> filter = Builders<TEntity>.Filter.And
                (
                    Builders<TEntity>.Filter.Where(query)
                );

        return await (await _collection.FindAsync(filter))
            .ToListAsync();
    }

    public async Task<List<TEntity>> FindManyAsync(List<Guid> ids)
    {
        if (ids.Count > 0)
        {
            FilterDefinition<TEntity> filter = Builders<TEntity>.Filter.And
                (
                    Builders<TEntity>.Filter.In(r => r.Id, ids)
                );

            return await (await _collection.FindAsync(filter)).ToListAsync();
        }

        return new List<TEntity>();
    }

    public Task InsertAsync(TEntity entity)
    {
        return _collection.InsertOneAsync(entity);
    }

    public Task InsertAsync(List<TEntity> entities)
    {
        if (entities.Count > 0)
        {
            return _collection.InsertManyAsync(entities);
        }

        return Task.CompletedTask;
    }

    public Task UpdateAsync(TEntity entity)
    {
        if (entity is IEntityWithVersion ewv)
        {
            ewv.SetNextVersion();
        }

        return _collection.ReplaceOneAsync(r=> r.Id == entity.Id, entity);
    }
}