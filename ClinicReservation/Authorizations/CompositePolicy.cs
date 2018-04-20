using AuthorizationCore;
using ClinicReservation.Models.Data;

namespace ClinicReservation.Authorizations
{
    public enum CompositeMethod
    {
        All,
        Any
    }
    public class CompositePolicy : IPolicy<User>
    {
        public CompositeMethod Method { get; }
        public string[] Policies { get; }

        public CompositePolicy(CompositeMethod method, params string[] policies)
        {
            Method = method;
            Policies = policies;
        }
    }

}
