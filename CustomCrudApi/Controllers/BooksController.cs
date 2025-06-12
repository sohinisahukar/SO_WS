using CustomCrudApi.Models;
using CustomCrudApi.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace CustomCrudApi.Controllers
{
    /// <summary>
    /// Controller for managing book operations with MongoDB.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class BooksController : ControllerBase
    {
        private readonly BookService _bookService;
        private readonly ILogger<BooksController> _logger;
        private static readonly Regex ObjectIdRegex = new("[0-9a-fA-F]{24}", RegexOptions.Compiled);

        /// <summary>
        /// Initializes a new instance of the <see cref="BooksController"/> class.
        /// </summary>
        /// <param name="bookService">The book service instance.</param>
        /// <param name="logger">The logger instance.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is null.</exception>
        public BooksController(BookService bookService, ILogger<BooksController> logger)
        {
            _bookService = bookService ?? throw new ArgumentNullException(nameof(bookService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }        /// <summary>
        /// Retrieves all books with optional pagination.
        /// </summary>
        /// <param name="page">The page number (starting from 1).</param>
        /// <param name="pageSize">The number of items per page (max 100).</param>
        /// <returns>A list of books.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<Book>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<Book>>> GetAsync(
            [FromQuery] int page = 1, 
            [FromQuery] int pageSize = 10)
        {
            try
            {
                _logger.LogInformation("Getting books with page {Page} and pageSize {PageSize}", page, pageSize);
                
                // Validate pagination parameters
                if (page < 1)
                {
                    _logger.LogWarning("Invalid page number: {Page}. Must be >= 1", page);
                    return BadRequest("Page number must be greater than or equal to 1.");
                }
                
                if (pageSize < 1 || pageSize > 100)
                {
                    _logger.LogWarning("Invalid page size: {PageSize}. Must be between 1 and 100", pageSize);
                    return BadRequest("Page size must be between 1 and 100.");
                }

                var books = await _bookService.GetAsync().ConfigureAwait(false);
                
                // Apply pagination
                var pagedBooks = books
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();
                
                _logger.LogInformation("Successfully retrieved {Count} books", pagedBooks.Count);
                return Ok(pagedBooks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving books");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }        /// <summary>
        /// Retrieves a book by its identifier.
        /// </summary>
        /// <param name="id">The book identifier.</param>
        /// <returns>The book if found; otherwise, NotFound.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Book), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Book>> GetAsync([FromRoute] string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                _logger.LogWarning("GetAsync called with null or empty id");
                return BadRequest("Book ID cannot be null or empty.");
            }

            if (!ObjectIdRegex.IsMatch(id))
            {
                _logger.LogWarning("GetAsync called with invalid id format: {Id}", id);
                return BadRequest("Invalid id format. Must be a 24-character hex string.");
            }

            try
            {
                _logger.LogInformation("Getting book with id: {Id}", id);
                
                var book = await _bookService.GetAsync(id).ConfigureAwait(false);
                if (book is null)
                {
                    _logger.LogWarning("Book with id {Id} not found", id);
                    return NotFound();
                }
                    
                _logger.LogInformation("Successfully retrieved book with id: {Id}", id);
                return Ok(book);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid argument for book id: {Id}", id);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving book with id: {Id}", id);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }        /// <summary>
        /// Creates a new book.
        /// </summary>
        /// <param name="newBook">The book to create.</param>
        /// <returns>A CreatedAtAction result with the created book.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(Book), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostAsync([FromBody] Book newBook)
        {
            if (newBook == null)
            {
                _logger.LogWarning("PostAsync called with null book");
                return BadRequest("Book cannot be null.");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("PostAsync called with invalid model state");
                return BadRequest(ModelState);
            }

            try
            {
                _logger.LogInformation("Creating new book with title: {Title}", newBook.Title);
                
                await _bookService.CreateAsync(newBook).ConfigureAwait(false);
                
                _logger.LogInformation("Successfully created book with id: {Id}", newBook.Id);
                return CreatedAtAction("Get", new { id = newBook.Id }, newBook);
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogWarning(ex, "Null argument provided for book creation");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating book");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }        /// <summary>
        /// Updates an existing book.
        /// </summary>
        /// <param name="id">The book identifier.</param>
        /// <param name="updatedBook">The updated book data.</param>
        /// <returns>A NoContent result if successful; otherwise, NotFound or BadRequest.</returns>
        [HttpPut("{id:length(24)}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateAsync([FromRoute] string id, [FromBody] Book updatedBook)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                _logger.LogWarning("UpdateAsync called with null or empty id");
                return BadRequest("Book ID cannot be null or empty.");
            }

            if (updatedBook == null)
            {
                _logger.LogWarning("UpdateAsync called with null book");
                return BadRequest("Book cannot be null.");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("UpdateAsync called with invalid model state");
                return BadRequest(ModelState);
            }

            try
            {
                _logger.LogInformation("Updating book with id: {Id}", id);
                
                var book = await _bookService.GetAsync(id).ConfigureAwait(false);
                if (book is null)
                {
                    _logger.LogWarning("Book with id {Id} not found for update", id);
                    return NotFound();
                }

                updatedBook.Id = book.Id;
                await _bookService.UpdateAsync(id, updatedBook).ConfigureAwait(false);
                
                _logger.LogInformation("Successfully updated book with id: {Id}", id);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid argument for book update with id: {Id}", id);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating book with id: {Id}", id);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }        /// <summary>
        /// Deletes a book.
        /// </summary>
        /// <param name="id">The book identifier.</param>
        /// <returns>A NoContent result if successful; otherwise, NotFound.</returns>
        [HttpDelete("{id:length(24)}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteAsync([FromRoute] string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                _logger.LogWarning("DeleteAsync called with null or empty id");
                return BadRequest("Book ID cannot be null or empty.");
            }

            try
            {
                _logger.LogInformation("Deleting book with id: {Id}", id);
                
                var book = await _bookService.GetAsync(id).ConfigureAwait(false);
                if (book is null)
                {
                    _logger.LogWarning("Book with id {Id} not found for deletion", id);
                    return NotFound();
                }

                await _bookService.RemoveAsync(id).ConfigureAwait(false);
                
                _logger.LogInformation("Successfully deleted book with id: {Id}", id);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid argument for book deletion with id: {Id}", id);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting book with id: {Id}", id);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
