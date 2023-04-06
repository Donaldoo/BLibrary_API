namespace LibraryAPI.Models.Dto
{
    public class BookUpdateDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string CreatedBy { get; set; }
        public int AuthorId { get; set; }
        public IFormFile? File { get; set; }
        public int[] CategoryId { get; set; }
    }
}
