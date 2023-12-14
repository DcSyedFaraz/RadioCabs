using Microsoft.AspNetCore.Identity;

namespace E_Project.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? Designation { get; set; }

        // Navigation property for the associated company
        public ICollection<Company> Companies { get; set; }
        public List<Advertisement> Advertisements { get; set; }
    }
}
