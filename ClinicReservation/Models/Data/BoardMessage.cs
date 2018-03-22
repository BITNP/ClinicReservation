using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicReservation.Models.Data
{
    public class BoardMessage
    {
        [Required, Key]
        public int Id { get; set; }

        [Required]
        public Reservation Reservation { get; set; }
        
        public User Poster { get; set; }

        public DateTime PostedTime { get; set; }

        [MaxLength(256)]
        public string Message { get; set; }

        public bool IsPublic { get; set; }
    }
}
