using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthenticationCore;
using ClinicReservation.Models.Data;
using ClinicReservation.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicReservation.Pages
{
    [AuthenticationRequired]
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
            if (!authenticationResult.IsAuthenticated)
            {
                return Redirect("/login");
            }

            User user = dbQuery.TryGetUserByName(authenticationResult.User.Name);
            if (user == null)
            {
                // create a user
                return Redirect("/newuser");
            }

            return Page();
        }
    }
}