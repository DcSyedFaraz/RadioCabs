using System.ComponentModel.DataAnnotations;

namespace E_Project.Models
{
    public class Feedback
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? MobileNumber { get; set; }
        public string? Email { get; set; }
        public string? City { get; set; }
        public string? Type { get; set; }
        public string? Description { get; set; }
    }

    public enum FeedbackType
    {
        Complaint,
        Suggestion,
        Compliment
    }

}
