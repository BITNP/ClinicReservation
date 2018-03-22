using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicReservation.Models.Data
{
    public class Feedback
    {
        [Required, Key]
        public int Id { get; set; }

        [Required]
        [Range(minimum: -2, maximum: 2)]
        public int Rate { get; set; }

        [MaxLength(256)]
        public string Content { get; set; }
        
        public int ReservationForeignKey { get; set; }
        public Reservation Reservation { get; set; }
    }
}
