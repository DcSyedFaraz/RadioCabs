using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_Project.Models
{
    public class Company
    {
        [Key]
        public int Id { get; set; }
        public string? UserId { get; set; }  // Foreign key to ApplicationUser table
        public string? Name { get; set; }
        public string? ContactPerson { get; set; }
        public string? Designation { get; set; }
        public string? Address { get; set; }
        public string? Mobile { get; set; }
        public string? Telephone { get; set; }
        public string? FaxNumber { get; set; }
        public string? Email { get; set; }
        public MembershipType MembershipType { get; set; }
        public PaymentType PaymentType { get; set; }
        // Add other properties as needed
        [ForeignKey("UserId")]
        public ApplicationUser? User { get; set; }
    }
    public enum MembershipType
    {
        Free = 0,
        Standard = 1,
        Premium = 2
    }

    public enum PaymentType
    {
        Monthly,
        Quarterly
    }

}
