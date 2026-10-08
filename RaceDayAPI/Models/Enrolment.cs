using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDayAPI.Models
{
    public class Enrolment
    {
        [Key]
        public int EnrolmentID { get; set; }

        [ForeignKey("Participant")]
        public int ParticipantID { get; set; }

        [ForeignKey("Event")]
        public int EventID { get; set; }

        [ForeignKey("Category")]
        public int CategoryID { get; set; }

        public DateTime RegistrationDate { get; set; } = DateTime.Now;

        public string Status { get; set; } = "Registered";

        public Participant Participant { get; set; } = null!;
        public Event Event { get; set; } = null!;
        public Category Category { get; set; } = null!;
        public Result? Result { get; set; }
    }
}