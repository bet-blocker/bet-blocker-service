using Infrastructure.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
namespace Infrastructure.Repositories;

public class MongoDbRepository : IMongoDbRepository
{
    private readonly IMongoCollection<BsonDocument> _collection;

    public MongoDbRepository(IConfiguration configuration)
    {
        var connectionString = configuration.GetSection("MongoDB:ConnectionString").Value;
        var databaseName = configuration.GetSection("MongoDB:DatabaseName").Value;
        var collectionName = configuration.GetSection("MongoDB:CollectionName").Value;

        var client = new MongoClient(connectionString);
        var database = client.GetDatabase(databaseName);
        _collection = database.GetCollection<BsonDocument>(collectionName);
    }

    public async Task InsertDocumentAsync(BsonDocument document)
    {
        await _collection.InsertOneAsync(document);
    }

    public async Task<List<BsonDocument>> GetAllDocumentsAsync()
    {
        return await _collection.Find(new BsonDocument()).ToListAsync();
    }
}