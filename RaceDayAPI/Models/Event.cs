using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDayAPI.Models
{
    public class Event
    {
        [Key]
        public int EventID { get; set; }

        [ForeignKey("Organiser")]
        public int OrganiserID { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public DateTime EventDate { get; set; }

        [Required]
        public string Location { get; set; } = string.Empty;

        public int? MaxParticipants { get; set; }

        public string Status { get; set; } = "Upcoming";

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public Organiser Organiser { get; set; } = null!;
        public ICollection<Category> Categories { get; set; } = new List<Category>();
    }
}
