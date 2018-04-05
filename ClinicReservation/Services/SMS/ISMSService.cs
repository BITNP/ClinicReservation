using System.Threading.Tasks;
using ClinicReservation.Models.Data;
using LocalizationCore;

namespace ClinicReservation.Services.SMS
{
    public interface ISMSService
    {
        Task SendAnsweredAsync(Reservation reservation);
        Task SendCreationSuccessAsync(Reservation reservation, ICultureExpression culture);
        Task SendReservationCancelledAsync(Reservation reservation);
        Task SendReservationClosedAsync(Reservation reservation);
        Task SendReservationCreatedAsync(Reservation reservation);
        Task SendReservationUpdatedAsync(Reservation reservation);
    }
}