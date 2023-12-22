using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_Project.Models
{
    public class Advertisement
    {
        [Key]
        public int Id { get; set; }
        public string? UserId { get; set; }  // Foreign key to ApplicationUser table
        public string? CompanyName { get; set; }
        public string? Designation { get; set; }
        public string? Address { get; set; }
        public string? Mobile { get; set; }
        public string? Telephone { get; set; }
        public string? FaxNumber { get; set; }
        public string? Email { get; set; }
        public string? Description { get; set; }
        public string? PaymentType { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser? User { get; set; }
    }

   

}
