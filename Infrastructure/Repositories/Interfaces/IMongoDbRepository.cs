using MongoDB.Bson;

namespace Infrastructure.Repositories.Interfaces;

public interface IMongoDbRepository
{
    Task InsertDocumentAsync(BsonDocument document);
    Task<List<BsonDocument>> GetAllDocumentsAsync();
}