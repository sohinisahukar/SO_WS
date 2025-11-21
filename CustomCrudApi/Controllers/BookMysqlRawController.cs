using CustomCrudApi.Models;
using CustomCrudApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace CustomCrudApi.Controllers
{
    /// <summary>
    /// Controller for managing book operations with MySQL using raw SQL queries.
    /// </summary>
    [ApiController]
    [Route("api/mysql/raw/[controller]")]
    [Produces("application/json")]
    public class BookMysqlRawController : ControllerBase
    {
        private readonly BookMysqlRawService _bookMysqlRawService;
        private readonly ILogger<BookMysqlRawController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="BookMysqlRawController"/> class.
        /// </summary>
        /// <param name="bookMysqlRawService">The book MySQL raw service instance.</param>
        /// <param name="logger">The logger instance.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is null.</exception>
        public BookMysqlRawController(BookMysqlRawService bookMysqlRawService, ILogger<BookMysqlRawController> logger)
        {
            _bookMysqlRawService = bookMysqlRawService ?? throw new ArgumentNullException(nameof(bookMysqlRawService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }        /// <summary>
        /// Retrieves all books from MySQL database with optional pagination.
        /// </summary>
        /// <param name="page">The page number (starting from 1).</param>
        /// <param name="pageSize">The number of items per page (max 100).</param>
        /// <returns>A collection of books.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Book>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<Book>>> GetAllAsync(
            [FromQuery] int page = 1, 
            [FromQuery] int pageSize = 10)
        {
            try
            {
                _logger.LogInformation("Getting books from MySQL with page {Page} and pageSize {PageSize}", page, pageSize);
                
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

                var books = await _bookMysqlRawService.GetAllAsync().ConfigureAwait(false);
                
                // Apply pagination
                var pagedBooks = books
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize);
                
                _logger.LogInformation("Successfully retrieved books from MySQL");
                return Ok(pagedBooks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving books from MySQL");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }        /// <summary>
        /// Retrieves a book by its identifier from MySQL database.
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

            try
            {
                _logger.LogInformation("Getting book from MySQL with id: {Id}", id);
                
                var book = await _bookMysqlRawService.GetByIdAsync(id).ConfigureAwait(false);
                if (book is null)
                {
                    _logger.LogWarning("Book with id {Id} not found in MySQL", id);
                    return NotFound();
                }
                    
                _logger.LogInformation("Successfully retrieved book from MySQL with id: {Id}", id);
                return Ok(book);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid argument for book id: {Id}", id);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving book from MySQL with id: {Id}", id);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }        /// <summary>
        /// Creates a new book in MySQL database.
        /// </summary>
        /// <param name="book">The book to create.</param>
        /// <returns>A CreatedAtAction result with the created book.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(Book), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateAsync([FromBody] Book book)
        {
            if (book == null)
            {
                _logger.LogWarning("CreateAsync called with null book");
                return BadRequest("Book cannot be null.");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("CreateAsync called with invalid model state");
                return BadRequest(ModelState);
            }

            try
            {
                _logger.LogInformation("Creating new book in MySQL with title: {Title}", book.Title);
                
                var created = await _bookMysqlRawService.CreateAsync(book).ConfigureAwait(false);
                
                _logger.LogInformation("Successfully created book in MySQL with id: {Id}", created.Id);
                return CreatedAtAction("Get", new { id = created.Id }, created);
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogWarning(ex, "Null argument provided for book creation");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating book in MySQL");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }        }

        /// <summary>
        /// Updates an existing book in MySQL database.
        /// </summary>
        /// <param name="id">The book identifier.</param>
        /// <param name="book">The updated book data.</param>
        /// <returns>A NoContent result if successful; otherwise, NotFound or BadRequest.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateAsync([FromRoute] string id, [FromBody] Book book)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                _logger.LogWarning("UpdateAsync called with null or empty id");
                return BadRequest("Book ID cannot be null or empty.");
            }

            if (book == null)
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
                _logger.LogInformation("Updating book in MySQL with id: {Id}", id);
                
                var success = await _bookMysqlRawService.UpdateAsync(id, book).ConfigureAwait(false);
                if (!success)
                {
                    _logger.LogWarning("Book with id {Id} not found for update in MySQL", id);
                    return NotFound();
                }
                    
                _logger.LogInformation("Successfully updated book in MySQL with id: {Id}", id);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid argument for book update with id: {Id}", id);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating book in MySQL with id: {Id}", id);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }        }

        /// <summary>
        /// Deletes a book from MySQL database.
        /// </summary>
        /// <param name="id">The book identifier.</param>
        /// <returns>A NoContent result if successful; otherwise, NotFound.</returns>
        [HttpDelete("{id}")]
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
                _logger.LogInformation("Deleting book from MySQL with id: {Id}", id);
                
                var success = await _bookMysqlRawService.RemoveAsync(id).ConfigureAwait(false);
                if (!success)
                {
                    _logger.LogWarning("Book with id {Id} not found for deletion in MySQL", id);
                    return NotFound();
                }
                    
                _logger.LogInformation("Successfully deleted book from MySQL with id: {Id}", id);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid argument for book deletion with id: {Id}", id);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting book from MySQL with id: {Id}", id);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}