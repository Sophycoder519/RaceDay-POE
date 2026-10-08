using System.ComponentModel.DataAnnotations;

namespace RaceDayAPI.Models
{
    public class User
    {
        [Key]
        public int UserID { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = string.Empty;

        [Required]
        public string FullName { get; set; } = string.Empty;

        public string? PhoneNumber { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public Organiser? Organiser { get; set; }
        public Participant? Participant { get; set; }
    }
}