using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDayAPI.Models
{
    public class Participant
    {
        [Key]
        public int ParticipantID { get; set; }

        [ForeignKey("User")]
        public int UserID { get; set; }

        public DateTime DateOfBirth { get; set; }

        public string? EmergencyContactName { get; set; }

        public string? EmergencyContactPhone { get; set; }

        public User User { get; set; } = null!;
    }
}