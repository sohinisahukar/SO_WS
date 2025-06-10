using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CustomCrudApi.Models
{
    public class Book
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [Key]                        // EF Core primary key
        [Column("id")]               // optional explicit column name
        public string? Id { get; set; }

        [BsonElement("title")]
        [Required]                   // EF Core NOT NULL
        public required string Title { get; set; }

        [BsonElement("author")]
        [Required]
        public required string Author { get; set; }

        [BsonElement("year")]
        public int Year { get; set; }
    }
}
