using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Newtonsoft.Json;

namespace LibraryAPI.Models.Dto
{
    public class AuthorUpdateDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Bio { get; set; }
        public string CreatedBy { get; set; }
    }
}
