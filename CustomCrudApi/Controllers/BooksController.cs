using CustomCrudApi.Models;
using CustomCrudApi.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace CustomCrudApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly BookService _bookService;
        private static readonly Regex ObjectIdRegex = new("[0-9a-fA-F]{24}");

        public BooksController(BookService bookService)
        {
            _bookService = bookService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Book>>> Get() =>
            await _bookService.GetAsync();

        [HttpGet("{id}")]
        public async Task<ActionResult<Book>> Get(string id)
        {
            if (!ObjectIdRegex.IsMatch(id))
                return BadRequest("Invalid id format. Must be a 24-character hex string.");

            var book = await _bookService.GetAsync(id);
            if (book is null)
                return NotFound();
            return book;
        }

        [HttpPost]
        public async Task<IActionResult> Post(Book newBook)
        {
            await _bookService.CreateAsync(newBook);
            return CreatedAtAction(nameof(Get), new { id = newBook.Id }, newBook);
        }

        [HttpPut("{id:length(24)}")]
        public async Task<IActionResult> Update(string id, Book updatedBook)
        {
            var book = await _bookService.GetAsync(id);
            if (book is null)
                return NotFound();
            updatedBook.Id = book.Id;
            await _bookService.UpdateAsync(id, updatedBook);
            return NoContent();
        }

        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> Delete(string id)
        {
            var book = await _bookService.GetAsync(id);
            if (book is null)
                return NotFound();
            await _bookService.RemoveAsync(id);
            return NoContent();
        }
    }
}
