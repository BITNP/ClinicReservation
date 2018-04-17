using AuthorizationCore.Attributes;
using ClinicReservation.Models.Data;

namespace ClinicReservation.Authorizations
{
    public class UserAuthorizationRequired : AuthorizationRequiredAttribute
    {
        public UserAuthorizationRequired(string expression, AuthorizationFailedAction failedAction = AuthorizationFailedAction.Return401, bool failedIfNotHandled = true) : base(expression, typeof(User), failedAction, failedIfNotHandled)
        {
        }
    }

}
