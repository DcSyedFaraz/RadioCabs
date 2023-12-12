using Microsoft.AspNetCore.Identity;

namespace E_Project.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? Designation { get; set; }
    }
}
