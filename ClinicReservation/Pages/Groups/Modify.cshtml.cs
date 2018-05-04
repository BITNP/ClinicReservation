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
using LocalizationCore;
using LocalizationCore.CodeMatching;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Primitives;

namespace ClinicReservation.Pages.Groups
{
    public class GroupActionName : ICodedItem, IEquatable<GroupActionName>, IComparable<GroupActionName>
    {
        private string code;
        public string Code => code;

        public string DefaultName => code;

        public string DisplayName { get; set; }

        public GroupActionName(string code)
        {
            this.code = code;
        }

        public bool Equals(GroupActionName other) => code == other.code;

        public int CompareTo(GroupActionName other) => code.CompareTo(other.code);
    }

    [AuthenticationRequired(AuthenticationPolicy.CASOnly, AuthenticationFailedAction.Return403)]
    [UserAuthorizationRequired(Policies.CanModifyGroups)]
    public class ModifyModel : CultureMatchingPageModel
    {
        private readonly IGroupActionProvider actionProvider;
        private readonly IDbQuery dbQuery;
        private readonly ICodeMatchingService service;

        public SortedSet<GroupActionName> AllowedPermissions { get; private set; }
        public SortedSet<GroupActionName> AllPermissions { get; private set; }
        public UserGroup Group { get; private set; }

        public ModifyModel(IGroupActionProvider actionProvider, IDbQuery dbQuery, ICodeMatchingService service)
        {
            this.actionProvider = actionProvider;
            this.dbQuery = dbQuery;
            this.service = service;
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
                name = actionProvider[permission.Action];
                allowed.Add(name);
            }
            AllowedPermissions = new SortedSet<GroupActionName>(allowed.Select(c => new GroupActionName(c)));
            AllPermissions = new SortedSet<GroupActionName>(all.Select(c => new GroupActionName(c)));
            service.Match<GroupActionName>(AllowedPermissions);
            service.Match<GroupActionName>(AllPermissions);
            Group = group;
            return Page();
        }

        public IActionResult OnPost(string code)
        {
            UserGroup group = dbQuery.TryGetGroupByCode(code);
            if (group == null)
                return CodeOnlyActionResult.Code404;
            EntityEntry<UserGroup> groupEntry = dbQuery.GetDbEntry(group);
            groupEntry.EnsureReferencesLoaded(nameof(group.Actions), nameof(group.Users));
            SortedSet<string> all = new SortedSet<string>(actionProvider.AllActionKeys);
            SortedSet<string> allowed = new SortedSet<string>();
            IFormCollection form = HttpContext.Request.Form;
            foreach (string key in all)
            {
                if (form.TryGetValue(key, out StringValues value) &&
                    (value == "on" || value == "yes" || (bool.TryParse(value, out bool isAllowed) && isAllowed)))
                {
                    allowed.Add(key);
                }
            }
            List<AllowedGroupAction> removing = new List<AllowedGroupAction>();
            foreach (AllowedGroupAction allowedAction in group.Actions)
            {
                string key = actionProvider[allowedAction.Action];
                if (allowed.Contains(key))
                {
                    allowed.Remove(key);
                    continue;
                }
                removing.Add(allowedAction);
            }
            foreach (AllowedGroupAction action in removing)
                group.Actions.Remove(action);
            foreach (string key in allowed)
            {
                if (actionProvider.TryGetAction(key, out GroupAction action))
                {
                    AllowedGroupAction groupAction = new AllowedGroupAction()
                    {
                        Group = group,
                        Action = action
                    };
                    group.Actions.Add(groupAction);
                }
            }
            groupEntry.State = EntityState.Modified;
            dbQuery.SaveChanges();
            return RedirectToPage("Index");
        }
    }
}