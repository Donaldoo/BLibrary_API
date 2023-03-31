using LibraryAPI.Data;
using LibraryAPI.Models;
using LibraryAPI.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace LibraryAPI.Controllers
{
    [Route("api/author")]
    [ApiController]
    public class AuthorController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private ApiResponse _response;

        public AuthorController(ApplicationDbContext db)
        {
            _db = db;
            _response = new ApiResponse();
        }

        [HttpGet]
        public async Task<IActionResult> GetAuthor()
        {
            _response.Result = await _db.Authors.Include(a => a.Books).ToListAsync();
            _response.StatusCode = HttpStatusCode.OK;
            return Ok(_response);
        }

        [HttpGet("{id:int}", Name = "GetAuthor")]
        public async Task<IActionResult> GetAuthor(int id)
        {
            if (id == 0)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                return BadRequest(_response);
            }
            Author author = _db.Authors.FirstOrDefault(u => u.Id == id);
            if (author == null)
            {
                _response.StatusCode=HttpStatusCode.NotFound;
                _response.IsSuccess =false;
                return NotFound(_response);
            }
            _response.Result = author;
            _response.StatusCode = HttpStatusCode.OK;
            return Ok(_response);
        }


        [Authorize]
        [HttpPost]
        public async Task<ActionResult<ApiResponse>> CreateAuthor([FromForm] AuthorCreateDTO authorCreateDTO)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (authorCreateDTO == null)
                    {
                        _response.StatusCode = HttpStatusCode.BadRequest;
                        _response.IsSuccess = false;
                        return BadRequest(_response);
                    }
                    // var userName = User.Identity.Name;
                    Author authorToCreate = new()
                    {
                        Name = authorCreateDTO.Name,
                        Bio = authorCreateDTO.Bio,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = User.Identity.Name
                    };
                    _db.Authors.Add(authorToCreate);
                    _db.SaveChanges();
                    _response.Result = authorToCreate;
                    _response.StatusCode = HttpStatusCode.Created;
                    return CreatedAtRoute("GetAuthor", new { id = authorToCreate.Id }, _response);
                }
                else
                {
                    _response.IsSuccess = false;
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.Message };
            }
            return _response;
        }
    }
}
