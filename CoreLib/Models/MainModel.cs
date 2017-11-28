using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreLib.Models
{
    public enum Repetition
    {
        Hourly = 0,
        Daily = 1,
        Monthly = 2
    }

    [Table("Identity")]
    public class Identity
    {
        public int ID { get; set; }
        public string PasswordHash { get; set; }
        public string Question { get; set; }
        public string AnswerHash { get; set; }
    }

    [Table("Schedule")]
    public class Schedule
    {
        public int ID { get; set; }
        [Required]
        public DayOfWeek Day { get; set; }

        [Required]
        public TimeSpan StartTime { get; set; }

        [StringLength(250)]
        public string Description { get; set; }

        [Required]
        [StringLength(500)]
        public string AudioLocation { get; set; }

        [Required]
        [StringLength(500)]
        public string PlayerLocation { get; set; }
    }
    [Table("Event")]
    public class Event
    {
        public int ID { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public TimeSpan StartTime { get; set; }

        [StringLength(250)]
        public string Description { get; set; }

        [Required]
        [StringLength(500)]
        public string AudioLocation { get; set; }

        [Required]
        [StringLength(500)]
        public string PlayerLocation { get; set; }
    }
    [Table("RepeatedSchedule")]
    public class RepeatedSchedule
    {
        public int ID { get; set; }

        [Required]
        public DateTime StartDateTime { get; set; }

        [Required]
        public Repetition Repetition { get; set; }

        [StringLength(250)]
        public string Description { get; set; }

        [Required]
        [StringLength(500)]
        public string AudioLocation { get; set; }

        [Required]
        [StringLength(500)]
        public string PlayerLocation { get; set; }
    }
}
