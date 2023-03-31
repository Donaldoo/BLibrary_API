using LibraryAPI.Data;
using LibraryAPI.Models;
using LibraryAPI.Models.Dto;
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
        private ApiResponse _response;

        public BookController(ApplicationDbContext db)
        {
            _db = db;
            _response = new ApiResponse();
        }

        [HttpGet]
        public async Task<IActionResult> GetBooks()
        {
            List<BookResponseDto> books = await _db.Books
                .Include(b => b.Author)
                .GroupJoin(
                    _db.BookCategories,
                    book => book.Id,
                    bookCategory => bookCategory.BookId,
                    (book, bookCategories) => new { Book = book, Categories = bookCategories.Select(bc => bc.Category) }
                )
                .Select(result => new BookResponseDto
                {
                    Book = result.Book,
                    Categories = result.Categories.ToList()
                })
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

            BookResponseDto? book = await _db.Books.Include(b => b.Author)
                .GroupJoin(
                    _db.BookCategories,
                    book => book.Id,
                    bookCategory => bookCategory.BookId,
                    (book, bookCategories) => new { Book = book, Categories = bookCategories.Select(bc => bc.Category) }
                )
                .Where(b => b.Book.Id == id)
                .Select(result => new BookResponseDto
                {
                    Book = result.Book,
                    Categories = result.Categories.ToList()
                }).FirstOrDefaultAsync();

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


        [HttpPost]
        public async Task<ActionResult<ApiResponse>> CreateBook([FromForm] BookCreateDTO bookCreateDTO)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (bookCreateDTO == null)
                    {
                        _response.StatusCode = HttpStatusCode.BadRequest;
                        _response.IsSuccess = false;
                        return BadRequest();
                    }
                    Book bookToCreate = new()
                    {
                        Name = bookCreateDTO.Name,
                        Description = bookCreateDTO.Description,
                        Image = bookCreateDTO.Image,
                        CreatedBy = "admin",
                        AuthorId = bookCreateDTO.AuthorId
                    };
                    _db.Books.Add(bookToCreate);
                    _db.SaveChanges();
                    _response.Result = bookToCreate;
                    _response.StatusCode = HttpStatusCode.Created;
                    return CreatedAtRoute("GetBook", new { id = bookToCreate.Id }, _response);
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
            return _response;
        }

    }
}
