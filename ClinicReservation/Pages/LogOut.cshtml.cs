using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthenticationCore;
using ClinicReservation.Handlers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicReservation.Pages
{
    [AuthenticationRequired(AuthenticationPolicy.CASOnly, AuthenticationFailedAction.CustomHandler)]
    [AuthenticationFailedHandler(typeof(RedirectHandler), "index")]
    public class LogOutModel : PageModel
    {
        private readonly IAuthenticationService service;

        public LogOutModel(IAuthenticationService service)
        {
            this.service = service;
        }
        public IActionResult OnGet()
        {
            service.RemoveCASSession();
            return service.CreateRedirectLogoutResult("/");
        }
    }
}