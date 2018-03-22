using System;
using System.ComponentModel.DataAnnotations;

namespace ClinicReservation.Models.Data
{
    public class DutySchedule
    {
        [Key, Required]
        public int Id { get; set; }

        [Required]
        public DayOfWeek Day { get; set; }

        [Required]
        public User User { get; set; }

    }
}
