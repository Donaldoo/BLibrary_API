namespace LibraryAPI.Models.Dto
{
    public class CategoryUpdateDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Priority { get; set; }
        public string CreatedBy { get; set; }
    }
}
