using Microsoft.AspNetCore.Mvc;

namespace booksReviews.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BookController : ControllerBase
    {
        private static bookClient _client = new bookClient();

        [HttpGet("book")]
        public async Task<IActionResult> Search(string query) => Ok(await _client.SearchBooksAsync(query));
        [HttpPost("addBook")]
        public async Task<IActionResult> Add(string title)
        {
            await _client.AddBook(title);
            return Ok(await _client.GetBookByTitle(title));
        }
        [HttpDelete("Book")]
        public async Task<IActionResult> DeleteBook(string query)
        {
            await _client.DeleteBook(query);
            return Ok();
        }
        [HttpGet("ByTitle")]
        public async Task<IActionResult> GetByTitle(string title) => Ok(await _client.GetBookByTitle(title));
        [HttpGet("AllBook")]
        public IActionResult Books() => Ok(_client.GetBooks());
        [HttpPut("Update")]
        public async Task<IActionResult> Update(string title, UpdateModel upBook)
        {
            await _client.UpdateBook(title, upBook);
            return Ok(await _client.GetBookByTitle(title));
        }
    }
}