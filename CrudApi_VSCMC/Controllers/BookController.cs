// A C# controller for Books with REST API endpoints using BookService
using CrudApi_VSCMC.Models;
using CrudApi_VSCMC.Services;
using Microsoft.AspNetCore.Mvc;

namespace CrudApi_VSCMC.Controllers;

[ApiController]
[Route("api/[controller]s")]  // Changed to 'books' to match test URLs
public class BookController : ControllerBase
{
    private readonly BookService _bookService;

    public BookController(BookService bookService)
    {
        _bookService = bookService;
    }

    [HttpGet]
    public async Task<ActionResult<List<Book>>> GetBooks()
    {
        return await _bookService.GetBooksAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Book>> GetBook(string id)
    {
        var book = await _bookService.GetBookByIdAsync(id);
        if (book == null)
            return NotFound();
        return book;
    }

    [HttpPost]
    public async Task<ActionResult<Book>> CreateBook(Book book)
    {
        await _bookService.CreateBookAsync(book);
        return CreatedAtAction(nameof(GetBook), new { id = book.Id }, book);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateBook(string id, Book book)
    {
        var existingBook = await _bookService.GetBookByIdAsync(id);
        if (existingBook == null)
            return NotFound();
            
        book.Id = id; // Ensure the ID matches
        await _bookService.UpdateBookAsync(id, book);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteBook(string id)
    {
        await _bookService.DeleteBookAsync(id);
        return NoContent();
    }
// This controller provides RESTful endpoints for managing books in a MongoDB database.
// It includes methods to get all books, get a book by ID, create a new book, update an existing book, and delete a book by ID.
}