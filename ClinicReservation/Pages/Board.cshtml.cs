using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthenticationCore;
using ClinicReservation.Handlers;
using ClinicReservation.Models.Data;
using ClinicReservation.Services;
using ClinicReservation.Services.Authentication;
using ClinicReservation.Services.Database;
using LocalizationCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicReservation.Pages
{
    [AuthenticationRequired(AuthenticationPolicy.CASOnly, AuthenticationFailedAction.CustomHandler)]
    [AuthenticationFailedHandler(typeof(RedirectHandler), "login")]
    public class BoardModel : CultureMatchingPageModel
    {
        private readonly IScopedUserAccessor userAccessor;

        public BoardModel(IScopedUserAccessor userAccessor)
        {
            this.userAccessor = userAccessor;
        }

        public IActionResult OnGet()
        {
            User user = userAccessor.User;
            if (user == null || !user.IsPersonalInformationFilled)
            {
                // create a user
                return Redirect("/newuser");
            }
            return Page();
        }
    }
}