using System.ComponentModel.DataAnnotations;

namespace CustomCrudApi.Models.DTOs
{
    /// <summary>
    /// Data Transfer Object for creating a new book.
    /// </summary>
    public class CreateBookRequest
    {
        /// <summary>
        /// Gets or sets the title of the book.
        /// </summary>
        [Required(ErrorMessage = "Title is required")]
        [StringLength(255, MinimumLength = 1, ErrorMessage = "Title must be between 1 and 255 characters")]
        public required string Title { get; set; }

        /// <summary>
        /// Gets or sets the author of the book.
        /// </summary>
        [Required(ErrorMessage = "Author is required")]
        [StringLength(255, MinimumLength = 1, ErrorMessage = "Author must be between 1 and 255 characters")]
        public required string Author { get; set; }

        /// <summary>
        /// Gets or sets the publication year of the book.
        /// </summary>
        [Range(1000, 9999, ErrorMessage = "Year must be between 1000 and 9999")]
        public int Year { get; set; }
    }

    /// <summary>
    /// Data Transfer Object for updating an existing book.
    /// </summary>
    public class UpdateBookRequest
    {
        /// <summary>
        /// Gets or sets the title of the book.
        /// </summary>
        [Required(ErrorMessage = "Title is required")]
        [StringLength(255, MinimumLength = 1, ErrorMessage = "Title must be between 1 and 255 characters")]
        public required string Title { get; set; }

        /// <summary>
        /// Gets or sets the author of the book.
        /// </summary>
        [Required(ErrorMessage = "Author is required")]
        [StringLength(255, MinimumLength = 1, ErrorMessage = "Author must be between 1 and 255 characters")]
        public required string Author { get; set; }

        /// <summary>
        /// Gets or sets the publication year of the book.
        /// </summary>
        [Range(1000, 9999, ErrorMessage = "Year must be between 1000 and 9999")]
        public int Year { get; set; }
    }

    /// <summary>
    /// Data Transfer Object for book response.
    /// </summary>
    public class BookResponse
    {
        /// <summary>
        /// Gets or sets the unique identifier for the book.
        /// </summary>
        public required string Id { get; set; }

        /// <summary>
        /// Gets or sets the title of the book.
        /// </summary>
        public required string Title { get; set; }

        /// <summary>
        /// Gets or sets the author of the book.
        /// </summary>
        public required string Author { get; set; }

        /// <summary>
        /// Gets or sets the publication year of the book.
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        /// Gets or sets the creation timestamp.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Data Transfer Object for paginated book results.
    /// </summary>
    public class PagedResult<T>
    {
        /// <summary>
        /// Gets or sets the list of items for the current page.
        /// </summary>
        public required IEnumerable<T> Items { get; set; }

        /// <summary>
        /// Gets or sets the current page number.
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// Gets or sets the page size.
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Gets or sets the total number of items.
        /// </summary>
        public int TotalItems { get; set; }

        /// <summary>
        /// Gets or sets the total number of pages.
        /// </summary>
        public int TotalPages { get; set; }

        /// <summary>
        /// Gets a value indicating whether there is a next page.
        /// </summary>
        public bool HasNextPage => Page < TotalPages;

        /// <summary>
        /// Gets a value indicating whether there is a previous page.
        /// </summary>
        public bool HasPreviousPage => Page > 1;
    }
}
