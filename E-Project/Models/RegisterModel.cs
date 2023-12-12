using System.ComponentModel.DataAnnotations;

namespace E_Project.Models
{
    public class RegisterModel
    {
        [Required(ErrorMessage ="User Name is required")]
        public string? UserName { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "EmailAddress is required")]
        public string? EmailAddress { get; set;}

        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set;}
        
        [Required(ErrorMessage = "Designation is required")]
        public string? Designation { get; set;}

    }
}
