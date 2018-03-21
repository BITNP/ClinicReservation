using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthenticationCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicReservation.Pages
{
    [AuthenticationRequired]
    public class BoardModel : PageModel
    {
        private readonly IAuthenticationService authenticationService;

        public BoardModel(IAuthenticationService authenticationService)
        {
            this.authenticationService = authenticationService;
        }

        public IActionResult OnGet([FromServices] IAuthenticationResult authenticationResult)
        {
            if (!authenticationResult.IsAuthenticated)
            {
                return Redirect("/Login");
            }
            return Page();
        }
    }
}