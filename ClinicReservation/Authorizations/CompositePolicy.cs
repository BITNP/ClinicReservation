using AuthorizationCore;
using ClinicReservation.Models.Data;

namespace ClinicReservation.Authorizations
{
    public class CompositePolicy : IPolicy<User>
    {
        public string[] Policies { get; }

        public CompositePolicy(params string[] policies)
        {
            Policies = policies;
        }
    }

}
