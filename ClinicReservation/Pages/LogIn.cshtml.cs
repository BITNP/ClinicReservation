using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthenticationCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicReservation.Pages
{
    public class LogInModel : PageModel
    {
        private readonly IAuthenticationService authenticationService;

        public LogInModel(IAuthenticationService authenticationService)
        {
            this.authenticationService = authenticationService;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            IAuthenticationResult authenticationResult = await authenticationService.CASAsync();
            if (authenticationResult.IsAuthenticated && authenticationResult.IsCAS)
            {
                // redirect to user's main page
                return Redirect("/Board");
            }
            else
            {
                // redirect to CAS
                return authenticationService.CreateRedirectCASResult();
            }
        }
    }
}