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
        var connectionString = Environment.GetEnvironmentVariable("MONGO_CONNECTION_STRING");
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new ArgumentNullException(nameof(connectionString), "A variável de ambiente 'MONGO_CONNECTION_STRING' não está configurada.");
        }

        var databaseName = Environment.GetEnvironmentVariable("MONGO_DATABASE_NAME");
        if (string.IsNullOrEmpty(databaseName))
        {
            throw new ArgumentNullException(nameof(databaseName), "A configuração 'MongoDB:DatabaseName' não está configurada no appsettings.");
        }

        var collectionName = Environment.GetEnvironmentVariable("MONGO_COLLECTION_NAME");
        if (string.IsNullOrEmpty(collectionName))
        {
            throw new ArgumentNullException(nameof(collectionName), "A configuração 'MongoDB:CollectionName' não está configurada no appsettings.");
        }
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