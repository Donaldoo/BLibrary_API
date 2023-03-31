using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryAPI.Models.Dto
{
    public class BookCreateDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public int AuthorId { get; set; }
    }
}
