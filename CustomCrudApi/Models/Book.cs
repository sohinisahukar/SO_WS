using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CustomCrudApi.Models
{
    /// <summary>
    /// Represents a book entity that supports both MongoDB and MySQL storage.
    /// </summary>
    public class Book
    {
        /// <summary>
        /// Gets or sets the unique identifier for the book.
        /// </summary>
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [Key]
        [Column("id")]
        public string? Id { get; set; }

        /// <summary>
        /// Gets or sets the title of the book.
        /// </summary>
        [BsonElement("title")]
        [Required]
        [MaxLength(255)]
        public required string Title { get; set; }

        /// <summary>
        /// Gets or sets the author of the book.
        /// </summary>
        [BsonElement("author")]
        [Required]
        [MaxLength(255)]
        public required string Author { get; set; }

        /// <summary>
        /// Gets or sets the publication year of the book.
        /// </summary>
        [BsonElement("year")]
        [Range(1000, 9999)]
        public int Year { get; set; }
    }
}
