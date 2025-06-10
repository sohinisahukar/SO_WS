using System.Collections.Generic;
using System.Threading.Tasks;
using CustomCrudApi.Models;
using CustomCrudApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace CustomCrudApi.Controllers
{
    [ApiController]
    [Route("api/mysql/raw/[controller]")]
    public class BookMysqlRawController : ControllerBase
    {
        private readonly BookMysqlRawService _svc;
        public BookMysqlRawController(BookMysqlRawService svc) => _svc = svc;

        [HttpGet]
        public Task<IEnumerable<Book>> GetAll() =>
            _svc.GetAllAsync();

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var book = await _svc.GetByIdAsync(id);
            return book is null ? NotFound() : Ok(book);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Book book)
        {
            var created = await _svc.CreateAsync(book);
            return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, Book book)
        {
            if (!await _svc.UpdateAsync(id, book))
                return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (!await _svc.RemoveAsync(id))
                return NotFound();
            return NoContent();
        }
    }
}