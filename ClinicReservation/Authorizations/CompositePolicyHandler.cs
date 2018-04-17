using AuthorizationCore;
using ClinicReservation.Models.Data;

namespace ClinicReservation.Authorizations
{
    public class CompositePolicyHandler : IPolicyHandler<User, CompositePolicy>
    {
        private readonly IAuthorizationService<User> service;
        public CompositePolicyHandler(IAuthorizationService<User> service)
        {
            this.service = service;
        }

        public PolicyResult OnAuthorization(User user, CompositePolicy policy)
        {
            PolicyResult result;
            foreach (string p in policy.Policies)
            {
                result = service.TryAuthorize(p);
                if (result != PolicyResult.Success)
                    return PolicyResult.Failed;
            }
            return PolicyResult.Success;
        }
    }

}
