namespace LibraryAPI.Models.Dto;

public class BookResponseDto
{
    public Book Book { get; set; }
    public List<Category> Categories { get; set; }
}