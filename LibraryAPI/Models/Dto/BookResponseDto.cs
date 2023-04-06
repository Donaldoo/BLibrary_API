using System.ComponentModel.DataAnnotations;

namespace LibraryAPI.Models.Dto;

public class BookResponseDto
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string CreatedBy { get; set; }
    public int AuthorId { get; set; }
    [Required]
    public IFormFile File { get; set; }
    public int[] CategoryId { get; set; }
}