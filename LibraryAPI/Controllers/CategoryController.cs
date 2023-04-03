using LibraryAPI.Data;
using LibraryAPI.Models;
using LibraryAPI.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;

namespace LibraryAPI.Controllers
{
    [Route("api/Category")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private ApiResponse _response;

        public CategoryController(ApplicationDbContext db)
        {
            _db = db;
            _response = new ApiResponse();
        }

        [HttpGet]
        public async Task<IActionResult> GetCategory()
        {
            _response.Result = _db.Categories;
            _response.StatusCode = HttpStatusCode.OK;
            return Ok(_response);
        }

        [HttpGet("{id:int}", Name = "GetCategory")]
        public async Task<IActionResult> GetCategory(int id)
        {
            if (id == 0)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                return BadRequest(_response);
            }
            Category category = _db.Categories.FirstOrDefault(u => u.Id == id);
            if (category == null)
            {
                _response.StatusCode = HttpStatusCode.NotFound;
                _response.IsSuccess = false;
                return NotFound(_response);
            }
            _response.Result = category;
            _response.StatusCode = HttpStatusCode.OK;
            return Ok(_response);
        }


        [HttpPost]
        public async Task<ActionResult<ApiResponse>> CreateCategory([FromBody] CategoryCreateDto categoryCreateDTO)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (categoryCreateDTO == null)
                    {
                        _response.StatusCode = HttpStatusCode.BadRequest;
                        _response.IsSuccess = false;
                        return BadRequest();
                    }
                    Category categoryToCreate = new()
                    {
                        Name = categoryCreateDTO.Name,
                        Priority = categoryCreateDTO.Priority,
                        CreatedAt = DateTime.UtcNow.ToString("dddd, dd MMMM yyyy"),
                        CreatedBy = categoryCreateDTO.CreatedBy
                };
                    _db.Categories.Add(categoryToCreate);
                    _db.SaveChanges();
                    _response.Result = categoryToCreate;
                    _response.StatusCode = HttpStatusCode.Created;
                    return CreatedAtRoute("GetCategory", new {id = categoryToCreate.Id}, _response);
                }
                else
                {
                    _response.IsSuccess = false;
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }


        [Authorize]
        [HttpPut("{id:int}")]
        public async Task<ActionResult<ApiResponse>> UpdateCategory(int id, [FromForm] CategoryUpdateDTO categoryUpdateDTO)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (categoryUpdateDTO == null || id != categoryUpdateDTO.Id)
                    {
                        _response.StatusCode = HttpStatusCode.BadRequest;
                        _response.IsSuccess = false;
                        return BadRequest();
                    }

                    Category categoryFromDb = await _db.Categories.FindAsync(id);
                    if (categoryFromDb == null)
                    {
                        _response.StatusCode = HttpStatusCode.BadRequest;
                        _response.IsSuccess = false;
                        return BadRequest();
                    }

                    categoryFromDb.Name = categoryUpdateDTO.Name;
                    categoryFromDb.Priority = categoryUpdateDTO.Priority;
                    categoryFromDb.CreatedAt = DateTime.UtcNow.ToString("dddd, dd MMMM yyyy");
                    categoryFromDb.CreatedBy = categoryUpdateDTO.CreatedBy;

                    _db.Categories.Update(categoryFromDb);
                    _db.SaveChanges();
                    _response.StatusCode = HttpStatusCode.NoContent;
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
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }


        [HttpDelete("{id:int}")]
        public async Task<ActionResult<ApiResponse>> DeleteCategory(int id)
        {
            try
            {
                if (id == 0)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    return BadRequest();
                }

                Category categoryFromDb = await _db.Categories.FindAsync(id);
                if (categoryFromDb == null)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    return BadRequest();
                }
                
                _db.Categories.Remove(categoryFromDb);
                _db.SaveChanges();
                _response.StatusCode = HttpStatusCode.NoContent;
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
