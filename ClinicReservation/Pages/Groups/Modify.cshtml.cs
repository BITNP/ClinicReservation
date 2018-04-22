using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthenticationCore;
using ClinicReservation.Authorizations;
using ClinicReservation.Handlers;
using ClinicReservation.Models;
using ClinicReservation.Models.Data;
using ClinicReservation.Services.Database;
using ClinicReservation.Services.Groups;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicReservation.Pages.Groups
{
    [AuthenticationRequired(AuthenticationPolicy.CASOnly, AuthenticationFailedAction.Return403)]
    [UserAuthorizationRequired(Policies.CanModifyGroups)]
    public class ModifyModel : PageModel
    {
        private readonly IGroupActionProvider actionProvider;
        private readonly IDbQuery dbQuery;

        public SortedSet<string> AllowedPermissions { get; private set; }
        public SortedSet<string> AllPermissions { get; private set; }
        public UserGroup Group { get; private set; }

        public ModifyModel(IGroupActionProvider actionProvider, IDbQuery dbQuery)
        {
            this.actionProvider = actionProvider;
            this.dbQuery = dbQuery;
        }

        public IActionResult OnGet(string code)
        {
            UserGroup group = dbQuery.TryGetGroupByCode(code);
            if (group == null)
                return CodeOnlyActionResult.Code404;

            dbQuery.GetDbEntry(group).EnsureReferencesLoaded(nameof(group.Actions), nameof(group.Users));
            string name;
            SortedSet<string> all = new SortedSet<string>(actionProvider.AllActionKeys);
            SortedSet<string> allowed = new SortedSet<string>();
            foreach (AllowedGroupAction permission in group.Actions)
            {
                name = actionProvider.GetKey(permission.Action);
                allowed.Add(name);
            }
            AllowedPermissions = allowed;
            AllPermissions = all;
            Group = group;
            return Page();
        }
    }
}