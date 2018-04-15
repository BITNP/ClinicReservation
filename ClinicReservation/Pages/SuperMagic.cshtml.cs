using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthenticationCore;
using ClinicReservation.Handlers;
using ClinicReservation.Models;
using ClinicReservation.Models.Data;
using ClinicReservation.Services.Authentication;
using ClinicReservation.Services.Database;
using ClinicReservation.Services.Groups;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicReservation.Pages
{
    [AuthenticationRequired(failedAction: AuthenticationFailedAction.CustomHandler)]
    [AuthenticationFailedHandlerAttribute(typeof(CustomReturnCodeHandler), 404)]
    public class SuperMagicModel : PageModel
    {
        private readonly IGroupPromptResolver resolver;
        private readonly IScopedUserAccessor userAccessor;
        private readonly IDbQuery dbQuery;

        public SuperMagicModel(IGroupPromptResolver resolver, IScopedUserAccessor userAccessor, IDbQuery dbQuery)
        {
            this.resolver = resolver;
            this.userAccessor = userAccessor;
            this.dbQuery = dbQuery;
        }

        public void OnGet()
        {

        }

        public void OnPost(string action, string code)
        {
            User user = userAccessor.User;
            dbQuery.GetDbEntry(user).EnsureReferencesLoaded(nameof(user.Groups));
            switch (action)
            {
                case "merge":
                    {
                        IReadOnlyList<UserGroup> groups = resolver.Resolve(code);
                        SortedSet<int> existingGroupIds = new SortedSet<int>(user.Groups.Select(link => link.GroupId));
                        foreach (UserGroup group in groups)
                        {
                            if (existingGroupIds.Contains(group.Id))
                                continue;
                            dbQuery.AddUserGroup(user, group);
                        }
                        dbQuery.SaveChanges();
                    }
                    break;
                case "replace":
                    {
                        IReadOnlyList<UserGroup> groups = resolver.Resolve(code);
                        SortedSet<int> existingGroupIds = new SortedSet<int>(user.Groups.Select(link => link.GroupId));
                        foreach (UserGroup group in groups)
                        {
                            if (existingGroupIds.Contains(group.Id))
                            {
                                existingGroupIds.Remove(group.Id);
                                continue;
                            }
                            dbQuery.AddUserGroup(user, group);
                        }
                        foreach (int id in existingGroupIds)
                        {
                            user.Groups.Remove(user.Groups.First(link => link.GroupId == id));
                        }
                        dbQuery.SaveChanges();
                    }
                    break;
            }

        }
    }
}