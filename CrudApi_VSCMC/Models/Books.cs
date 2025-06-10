// A C# model class for MongoDB with Id, Title, Author, Year using Bson attributes
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CrudApi_VSCMC.Models;

[BsonIgnoreExtraElements]
public class Book
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;

    [BsonElement("title")]
    public string Title { get; set; } = null!;

    [BsonElement("author")]
    public string Author { get; set; } = null!;

    [BsonElement("year")]
    public int Year { get; set; }
}
