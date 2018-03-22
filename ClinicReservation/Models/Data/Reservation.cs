using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ClinicReservation.Models.Data
{
    public enum ReservationState
    {
        Created = 0,
        Cancelled = 1,
        Accepted = 2,
        Completed = 3,
        ClosedWithoutComplete = 4
    }
    public class Reservation
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public User Poster { get; set; }

        public User Duty { get; set; }

        [Required]
        public Category Category { get; set; }

        [Required]
        public Location Location { get; set; }

        [MaxLength(8)]
        public string LastUsedCulture { get; set; }

        [Required]
        public ReservationState State { get; set; }

        [MaxLength(512)]
        [MinLength(5)]
        public string Detail { get; set; }


        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}")]
        public DateTime CreatedDate { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}")]
        public DateTime LastUserModifiedDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime ReservationDate { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}")]
        public DateTime LastActionDate { get; set; }

        public ICollection<BoardMessage> Messages { get; set; }

        public Feedback Feedback { get; set; }
    }
}
