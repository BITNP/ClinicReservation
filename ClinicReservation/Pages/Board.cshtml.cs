using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthenticationCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicReservation.Pages
{
    public class BoardModel : PageModel
    {
        private readonly IAuthenticationService authenticationService;

        public BoardModel(IAuthenticationService authenticationService)
        {
            this.authenticationService = authenticationService;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            IAuthenticationResult authenticationResult = await authenticationService.CASAsync();
            if (!authenticationResult.IsAuthenticated || !authenticationResult.IsCAS)
                return Unauthorized();


            return Page();
        }
    }
}