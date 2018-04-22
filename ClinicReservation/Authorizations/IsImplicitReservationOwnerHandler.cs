using AuthorizationCore;
using ClinicReservation.Models;
using ClinicReservation.Models.Data;
using ClinicReservation.Services.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace ClinicReservation.Authorizations
{
    public class IsImplicitReservationOwnerHandler : IPolicyHandler<User, IsImplicitReservationOwnerPolicy>
    {
        private readonly IDbQuery query;
        private readonly IHttpContextAccessor httpContextAccessor;

        public IsImplicitReservationOwnerHandler(IDbQuery query, IHttpContextAccessor httpContextAccessor)
        {
            this.query = query;
            this.httpContextAccessor = httpContextAccessor;
        }

        public PolicyResult OnAuthorization(User user, IsImplicitReservationOwnerPolicy policy)
        {
            if (user == null)
                return PolicyResult.Failed;

            int id;
            Reservation reservation;
            HttpRequest request = httpContextAccessor.HttpContext.Request;
            if (request.HasFormContentType)
            {
                foreach (string key in policy.IdKeys)
                {
                    if (request.Query.TryGetValue(key, out StringValues idValues))
                    {
                        string idValue = idValues;
                        if (int.TryParse(idValue, out id))
                        {
                            reservation = query.TryGetReservation(id);
                            if (reservation == null)
                                return PolicyResult.Failed;
                            query.GetDbEntry(reservation).EnsureReferencesLoaded(false);
                            return CheckReservation(user, reservation);
                        }
                    }
                    else if (request.Form.TryGetValue(key, out idValues))
                    {
                        string idValue = idValues;
                        if (int.TryParse(idValue, out id))
                        {
                            reservation = query.TryGetReservation(id);
                            if (reservation == null)
                                return PolicyResult.Failed;
                            query.GetDbEntry(reservation).EnsureReferencesLoaded(false);
                            return CheckReservation(user, reservation);
                        }
                    }
                }
            }
            else
            {
                foreach (string key in policy.IdKeys)
                {
                    if (request.Query.TryGetValue(key, out StringValues idValues))
                    {
                        string idValue = idValues;
                        if (int.TryParse(idValue, out id))
                        {
                            reservation = query.TryGetReservation(id);
                            if (reservation == null)
                                return PolicyResult.Failed;
                            query.GetDbEntry(reservation).EnsureReferencesLoaded(false);
                            return CheckReservation(user, reservation);
                        }
                    }
                }
            }

            return PolicyResult.Failed;
        }
        private PolicyResult CheckReservation(User user, Reservation reservation)
        {
            query.GetDbEntry(reservation).EnsureReferencesLoaded(false);
            if (reservation.Poster == user)
                return PolicyResult.Success;
            else
                return PolicyResult.Failed;
        }
    }
}
