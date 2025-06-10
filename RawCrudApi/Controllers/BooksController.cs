using RawCrudApi.Models;
using RawCrudApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace RawCrudApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly BookService _bookService;

    public BooksController(BookService bookService) => _bookService = bookService;

    [HttpGet]
    public ActionResult<List<Book>> Get() => _bookService.Get();

    [HttpGet("{id}")]
    public ActionResult<Book> Get(string id)
    {
        var book = _bookService.Get(id);
        return book is null ? NotFound() : book;
    }

    [HttpPost]
    public ActionResult Create(Book book)
    {
        _bookService.Create(book);
        return CreatedAtAction(nameof(Get), new { id = book.Id }, book);
    }

    [HttpPut("{id}")]
    public IActionResult Update(string id, Book updatedBook)
    {
        var book = _bookService.Get(id);
        if (book is null) return NotFound();

        updatedBook.Id = id;
        _bookService.Update(id, updatedBook);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(string id)
    {
        var book = _bookService.Get(id);
        if (book is null) return NotFound();

        _bookService.Delete(id);
        return NoContent();
    }
}
