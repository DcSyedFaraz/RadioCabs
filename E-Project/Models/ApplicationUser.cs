using Microsoft.AspNetCore.Identity;

namespace E_Project.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? Designation { get; set; }

        // Navigation property for the associated company
        public ICollection<Company>? Companies { get; set; }
        public ICollection<Advertisement>? Advertisements { get; set; }
        public ICollection<Driver>? Drivers { get; set; }
    }
}
