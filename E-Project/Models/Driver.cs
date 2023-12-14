using System.ComponentModel.DataAnnotations;

namespace E_Project.Models
{
    public class Driver
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? ContactPerson { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Mobile { get; set; }
        public string? Telephone { get; set; }
        public string? Email { get; set; }
        public int Experience { get; set; }
        public string? Description { get; set; }
        public PaymentType PaymentType { get; set; }
        // Add other properties as needed
    }

}
