using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthenticationCore;
using ClinicReservation.Authorizations;
using LocalizationCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicReservation.Pages
{
    [AuthenticationRequired(AuthenticationPolicy.CASOnly, AuthenticationFailedAction.Return403)]
    [UserAuthorizationRequired(Policies.CanModifyGroups)]
    public class GroupsModel : CultureMatchingPageModel
    {
        public void OnGet()
        {

        }
    }
}