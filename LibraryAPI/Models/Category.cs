using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryAPI.Models
{
    public class Category
    { 
        public int Id { get; set; }
        public string Name { get; set; }
        public string Priority { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public ICollection<BookCategory> BookCategories { get; set; }
    }
}
