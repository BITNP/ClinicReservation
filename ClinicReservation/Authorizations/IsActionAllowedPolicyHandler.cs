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
    public class IsActionAllowedPolicyHandler : IPolicyHandler<User, IsActionAllowedPolicy>
    {
        private readonly IDbQuery query;

        public IsActionAllowedPolicyHandler(IDbQuery query)
        {
            this.query = query;
        }

        public PolicyResult OnAuthorization(User user, IsActionAllowedPolicy policy)
        {
            if (user == null)
                return PolicyResult.Failed;

            if (policy.Actions.Count <= 0)
                return PolicyResult.Success;

            SortedSet<GroupAction> targetActions = new SortedSet<GroupAction>(policy.Actions);
            query.GetDbEntry(user).EnsureReferencesLoaded(true);
            ICollection<UserGroupUser> groupLinks = user.Groups;
            UserGroup group;
            foreach (UserGroupUser links in groupLinks)
            {
                query.GetDbEntry(links).EnsureReferencesLoaded(false);
                group = links.Group;
                query.GetDbEntry(group).EnsureReferencesLoaded(true);
                foreach (AllowedGroupAction action in group.Actions)
                {
                    if (targetActions.Contains(action.Action))
                        targetActions.Remove(action.Action);
                }
                if (targetActions.Count <= 0)
                    return PolicyResult.Success;
            }
            if (targetActions.Count <= 0)
                return PolicyResult.Success;
            else
                return PolicyResult.Failed;
        }
    }
}
