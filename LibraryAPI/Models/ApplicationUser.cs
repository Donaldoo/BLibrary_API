using Microsoft.AspNetCore.Identity;

namespace LibraryAPI.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
    }
}
