using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthenticationCore;
using AuthorizationCore.Attributes;
using ClinicReservation.Authorizations;
using ClinicReservation.Models.Data;
using ClinicReservation.Services.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ClinicReservation.Pages.Groups
{
    [AuthenticationRequired(AuthenticationPolicy.CASOnly, AuthenticationFailedAction.Return403)]
    [UserAuthorizationRequired(Policies.CanModifyGroups)]
    public class IndexModel : PageModel
    {
        private readonly DataDbContext dbContext;

        public IEnumerable<UserGroup> Groups { get; private set; }

        public IndexModel(DataDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public void OnGet()
        {
            IEnumerable<UserGroup> groups = dbContext.UserGroups
                                                     .Include(nameof(UserGroup.Users))
                                                     .Include(nameof(UserGroup.Actions));
            Groups = groups;
        }
    }
}