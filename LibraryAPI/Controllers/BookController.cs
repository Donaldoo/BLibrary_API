using LibraryAPI.Data;
using LibraryAPI.Models;
using LibraryAPI.Models.Dto;
using LibraryAPI.Services;
using LibraryAPI.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace LibraryAPI.Controllers
{
    [Route("api/Book")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IBlobService _blobService;
        private ApiResponse _response;

        public BookController(ApplicationDbContext db, IBlobService blobService)
        {
            _db = db;
            _blobService = blobService;
            _response = new ApiResponse();
        }

        [HttpGet]
        public async Task<IActionResult> GetBooks()
        {
            var books = await _db.Books
                .Include(b => b.BookCategories)
                .ThenInclude(c => c.Category)
                .ToListAsync();

            _response.Result = books;
            _response.StatusCode = HttpStatusCode.OK;
            return Ok(_response);
        }

        [HttpGet("{id:int}", Name = "GetBook")]
        public async Task<IActionResult> GetBook(int id)
        {
            if (id == 0)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                return BadRequest(_response);
            }

            var book = await _db.Books
                .Include(b => b.BookCategories)
                .ThenInclude(c => c.Category)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (book == null)
            {
                _response.StatusCode = HttpStatusCode.NotFound;
                _response.IsSuccess = false;
                return NotFound(_response);
            }
            _response.Result = book;
            _response.StatusCode = HttpStatusCode.OK;
            return Ok(_response);
        }


        [HttpGet("{id}/categories")]
        public async Task<ActionResult<IEnumerable<Category>>> GetBookCategories(int id)
        {
            var book = await _db.Books
                .Include(b => b.BookCategories)
                .ThenInclude(bc => bc.Category)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (book == null )
            {
                return NotFound();
            }
            var categories = book.BookCategories.Select(c => c.Category).ToList();
            return Ok(categories);
        }


        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ApiResponse>> CreateBook([FromForm] BookResponseDto bookResponseDto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (bookResponseDto == null)
                    {
                        _response.StatusCode = HttpStatusCode.BadRequest;
                        _response.IsSuccess = false;
                        return BadRequest();
                    }

                    string filename = $"{Guid.NewGuid()}{Path.GetExtension(bookResponseDto.File.FileName)}";

                    var book = new Book()
                    {
                        Name = bookResponseDto.Name,
                        Description = bookResponseDto.Description,
                        Image = await _blobService.UploadBlob(filename, SD.SD_Storage_Container, bookResponseDto.File),
                        CreatedAt = DateTime.UtcNow.ToString("dddd, dd MMMM yyyy"),
                        CreatedBy = bookResponseDto.CreatedBy,
                        AuthorId = bookResponseDto.AuthorId
                    };
                    foreach (var item in bookResponseDto.CategoryId)
                    {
                        book.BookCategories.Add(new BookCategory()
                        {
                            Book = book,
                            CategoryId = item,
                        });
                    }
                    _db.Books.Add(book);
                    _db.SaveChanges();
                    _response.Result = book;
                    _response.StatusCode = HttpStatusCode.Created;
                    return CreatedAtRoute("GetBook", new { id = book.Id }, _response);
                }
                else
                {
                    _response.IsSuccess = false;
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.Message };
            }
            return Ok(_response);
        }

        [HttpPut("{id:int}")]
        [Authorize]
        public async Task<ActionResult<ApiResponse>> UpdateBook(int id, [FromForm] BookUpdateDto bookUpdateDto)
        {
            if (id == 0)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                return BadRequest(_response);
            }
            var book = await _db.Books
                .Include(b => b.BookCategories)
                .ThenInclude(c => c.Category)
                .FirstOrDefaultAsync(b => b.Id == id);
            if (book == null)
            {
                _response.StatusCode = HttpStatusCode.NotFound;
                _response.IsSuccess = false;
                return BadRequest(_response);
            }
            try
            {
                if (ModelState.IsValid)
                {
                    if (bookUpdateDto == null)
                    {
                        _response.StatusCode = HttpStatusCode.BadRequest;
                        _response.IsSuccess = false;
                        return BadRequest();
                    }
                    book.Id = bookUpdateDto.Id;
                    book.Name = bookUpdateDto.Name;
                    book.Description = bookUpdateDto.Description;
                    if (bookUpdateDto.File != null && bookUpdateDto.File.Length > 0)
                    {
                        string filename = $"{Guid.NewGuid()}{Path.GetExtension(bookUpdateDto.File.FileName)}";
                        await _blobService.DeleteBlob(book.Image.Split('/').Last(), SD.SD_Storage_Container);
                        book.Image = await _blobService.UploadBlob(filename, SD.SD_Storage_Container, bookUpdateDto.File);
                    }
                    book.AuthorId = bookUpdateDto.AuthorId;
                    book.CreatedAt = DateTime.Now.ToString("dddd, dd MMMM yyyy");
                    book.CreatedBy = bookUpdateDto.CreatedBy;
                    book.BookCategories.Clear();
                    foreach (var item in bookUpdateDto.CategoryId)
                    {
                        book.BookCategories.Add(new BookCategory()
                        {
                            Book = book,
                            CategoryId = item,
                        });
                    }
                    _db.Books.Update(book);
                    _db.SaveChanges();
                    _response.Result = book;
                    _response.StatusCode = HttpStatusCode.OK;
                    return Ok(_response);
                }
                else
                {
                    _response.IsSuccess = false;
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.Message };
            }
            return Ok(_response);
        }

        [HttpDelete("{id:int}")]
        [Authorize]
        public async Task<ActionResult<ApiResponse>> DeleteBook(int id)
        {
            try
            {
                if (id == 0)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    return BadRequest(_response);
                }
                var book = await _db.Books
                    .Include(b => b.BookCategories)
                    .ThenInclude(c => c.Category)
                    .FirstOrDefaultAsync(b => b.Id == id);
                if (book == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.IsSuccess = false;
                    return BadRequest(_response);
                }
                await _blobService.DeleteBlob(book.Image.Split('/').Last(), SD.SD_Storage_Container);
                int milliseconds = 1000;
                Thread.Sleep(milliseconds);

                _db.Books.Remove(book);
                _db.SaveChanges();
                _response.StatusCode=HttpStatusCode.NoContent;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }
    }
}
