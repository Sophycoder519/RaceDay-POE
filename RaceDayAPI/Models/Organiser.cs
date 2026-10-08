using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDayAPI.Models
{
    public class Organiser
    {
        [Key]
        public int OrganiserID { get; set; }

        [ForeignKey("User")]
        public int UserID { get; set; }

        public string? OrganisationName { get; set; }

        public string? BusinessPhone { get; set; }

        public User User { get; set; } = null!;
    }
}
