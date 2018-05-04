using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthenticationCore;
using ClinicReservation.Authorizations;
using ClinicReservation.Handlers;
using LocalizationCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicReservation.Pages
{
    [AuthenticationRequired(AuthenticationPolicy.CASOnly, AuthenticationFailedAction.CustomHandler)]
    [AuthenticationFailedHandler(typeof(RedirectHandler), "login")]
    [UserAuthorizationRequired(Policies.CanCreateReservation)]
    public class MineModel : CultureMatchingPageModel
    {

        public void OnGet()
        {

        }
    }
}