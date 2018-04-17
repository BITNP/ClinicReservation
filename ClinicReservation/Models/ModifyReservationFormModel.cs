using ClinicReservation.Models.Data;
using ClinicReservation.Validates;
using System.ComponentModel.DataAnnotations;

namespace ClinicReservation.Models
{
    public class ModifyReservationFormModel : NewReservationFormModel
    {
        [Required]
        [Reservation]
        public int Reservation { get; set; }

        public Reservation ReservationInstance { get; set; }

        public ModifyReservationFormModel() { }
        public ModifyReservationFormModel(Reservation reservation)
        {
            Reservation = reservation.Id;
            ReservationInstance = reservation;
            Detail = reservation.Detail;
            Location = reservation.Location.Code;
            Category = reservation.Category.Code;
            BookDate = reservation.ReservationDate.ToString("yyyy/mm/dd");
            LocationInstance = reservation.Location;
            CategoryInstance = reservation.Category;
            BookDateInstance = reservation.ReservationDate;
        }
    }
}