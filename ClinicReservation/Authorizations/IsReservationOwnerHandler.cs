using AuthorizationCore;
using ClinicReservation.Models;
using ClinicReservation.Models.Data;
using ClinicReservation.Services.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClinicReservation.Authorizations
{
    public class IsReservationOwnerHandler : IPolicyHandler<User, Reservation, IsReservationOwnerPolicy>
    {
        private readonly IDbQuery query;

        public IsReservationOwnerHandler(IDbQuery query)
        {
            this.query = query;
        }

        public PolicyResult OnAuthorization(User user, Reservation target, IsReservationOwnerPolicy policy)
        {
            if (target == null)
                return PolicyResult.Failed;

            query.GetDbEntry(target).EnsureReferencesLoaded(false);
            if (target.Poster == user)
                return PolicyResult.Success;
            else
                return PolicyResult.Failed;
        }
    }
}
