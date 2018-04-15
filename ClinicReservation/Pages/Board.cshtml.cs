using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthenticationCore;
using ClinicReservation.Handlers;
using ClinicReservation.Models.Data;
using ClinicReservation.Services;
using ClinicReservation.Services.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicReservation.Pages
{
    [AuthenticationRequired(AuthenticationPolicy.CASOnly, AuthenticationFailedAction.CustomHandler)]
    [AuthenticationFailedHandlerAttribute(typeof(RedirectHandler), "login")]
    public class BoardModel : PageModel
    {
        private readonly IAuthenticationService authenticationService;
        private readonly IDbQuery dbQuery;

        public BoardModel(IAuthenticationService authenticationService, IDbQuery dbQuery)
        {
            this.authenticationService = authenticationService;
            this.dbQuery = dbQuery;
        }

        public IActionResult OnGet([FromServices] IAuthenticationResult authenticationResult)
        {
            User user = dbQuery.TryGetUser(authenticationResult.User);
            if (user == null || !user.IsPersonalInformationFilled)
            {
                // create a user
                return Redirect("/newuser");
            }

            return Page();
        }
    }
}