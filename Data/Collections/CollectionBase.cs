using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Data.Collections;

public class CollectionBase
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}