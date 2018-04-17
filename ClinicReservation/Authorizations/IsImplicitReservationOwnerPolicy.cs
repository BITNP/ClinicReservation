using AuthorizationCore;
using ClinicReservation.Models.Data;

namespace ClinicReservation.Authorizations
{
    public class IsImplicitReservationOwnerPolicy : IPolicy<User>
    {
        public string[] IdKeys { get; }

        public IsImplicitReservationOwnerPolicy(params string[] idKeys)
        {
            IdKeys = idKeys;
        }
    }
}
