using AuthorizationCore;
using ClinicReservation.Models.Data;
using System;
using System.Collections.Generic;

namespace ClinicReservation.Authorizations
{
    public class IsActionAllowedPolicy : IPolicy<User>
    {
        public SortedSet<GroupAction> Actions { get; }

        public IsActionAllowedPolicy(params GroupAction[] actions)
        {
            Actions = new SortedSet<GroupAction>(actions);
        }
    }

}