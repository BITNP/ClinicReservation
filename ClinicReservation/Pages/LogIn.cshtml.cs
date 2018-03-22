using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthenticationCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicReservation.Pages
{
    [AuthenticationRequired(AuthenticationPolicy.All, AuthenticationFailedAction.RedirectCAS)]
    public class LogInModel : PageModel
    {
        public IActionResult OnGet()
        {
            return Redirect("/Board");
        }
    }
}