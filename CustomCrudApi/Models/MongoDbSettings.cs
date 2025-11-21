namespace CustomCrudApi.Models
{
    /// <summary>
    /// Configuration settings for MongoDB connection.
    /// </summary>
    public class MongoDbSettings
    {
        /// <summary>
        /// Gets or sets the MongoDB connection string.
        /// </summary>
        public required string ConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the name of the MongoDB database.
        /// </summary>
        public required string DatabaseName { get; set; }

        /// <summary>
        /// Gets or sets the name of the books collection in MongoDB.
        /// </summary>
        public required string BooksCollectionName { get; set; }
    }
}
