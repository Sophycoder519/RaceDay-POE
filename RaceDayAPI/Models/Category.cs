using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDayAPI.Models
{
    public class Category
    {
        [Key]
        public int CategoryID { get; set; }

        [ForeignKey("Event")]
        public int EventID { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public decimal Distance { get; set; }

        public decimal EntryFee { get; set; }

        public TimeSpan StartTime { get; set; }

        public int? MaxParticipants { get; set; }

        public Event Event { get; set; } = null!;
    }
}