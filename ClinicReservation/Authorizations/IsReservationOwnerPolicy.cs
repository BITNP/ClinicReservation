using AuthorizationCore;
using ClinicReservation.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClinicReservation.Authorizations
{
    public class IsReservationOwnerPolicy : IPolicy<User, Reservation>
    {

    }
}
