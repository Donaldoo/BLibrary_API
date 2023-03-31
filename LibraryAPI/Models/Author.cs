using System.Text.Json.Serialization;

namespace LibraryAPI.Models
{
    public class Author
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Bio { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        
        public List<Book> Books { get; set; }
    }
}
