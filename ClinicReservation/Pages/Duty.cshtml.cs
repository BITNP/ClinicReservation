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
    [AuthenticationRequired(failedAction: AuthenticationFailedAction.CustomHandler)]
    [AuthenticationFailedHandlerAttribute(typeof(RedirectHandler), "/login")]
    public class DutyModel : PageModel
    {
        public void OnGet()
        {

        }
    }
}