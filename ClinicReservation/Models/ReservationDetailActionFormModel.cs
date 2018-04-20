using ClinicReservation.Models.Data;
using ClinicReservation.Validates;
using System.ComponentModel.DataAnnotations;

namespace ClinicReservation.Models
{
    public class ReservationDetailActionFormModel
    {
        [Required]
        [Reservation]
        public int Reservation { get; set; }

        [Required]
        public string Action { get; set; }

        public string Parameter { get; set; }

        public Reservation ReservationInstance { get; set; }
    }
}