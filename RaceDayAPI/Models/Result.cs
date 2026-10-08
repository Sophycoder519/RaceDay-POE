using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDayAPI.Models
{
    public class Result
    {
        [Key]
        public int ResultID { get; set; }

        [ForeignKey("Enrolment")]
        public int EnrolmentID { get; set; }

        public TimeSpan FinishTime { get; set; }

        public int? OverallPosition { get; set; }

        public int? CategoryPosition { get; set; }

        public bool IsDisqualified { get; set; } = false;

        public string? Notes { get; set; }

        public Enrolment Enrolment { get; set; } = null!;
    }
}