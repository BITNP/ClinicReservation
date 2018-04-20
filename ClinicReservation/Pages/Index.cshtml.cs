using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthenticationCore;
using LocalizationCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicReservation.Pages
{
    [AuthenticationRequired]
    public class IndexModel : CultureMatchingPageModel
    {
        private readonly IAuthenticationService authenticationService;

        public IndexModel(IAuthenticationService authenticationService)
        {
            this.authenticationService = authenticationService;
        }


        public IActionResult OnGet([FromServices]IAuthenticationResult authenticationResult)
        {
            if (authenticationResult.IsAuthenticated)
                return RedirectToPage("board");

            return Page();
        }
    }
}