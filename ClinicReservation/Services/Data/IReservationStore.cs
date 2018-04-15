using ClinicReservation.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClinicReservation.Services.Data
{
    public interface IReservationStore
    {
        Reservation Reservation { get; set; }
    }

    internal sealed class ReservationStore : IReservationStore
    {
        public Reservation Reservation { get; set; }
    }
}
