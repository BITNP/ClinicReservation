using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthenticationCore;
using ClinicReservation.Authorizations;
using ClinicReservation.Handlers;
using ClinicReservation.Models.Data;
using ClinicReservation.Services;
using ClinicReservation.Services.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicReservation.Pages
{
    [AuthenticationRequired(AuthenticationPolicy.CASOnly, AuthenticationFailedAction.Return403)]
    [UserAuthorizationRequired(Policies.CanChangeServiceState)]
    public class StateModel : PageModel
    {
        private readonly IServiceState state;
        private readonly IDbQuery query;

        public StateModel(IServiceState state, IDbQuery query)
        {
            this.state = state;
            this.query = query;
        }

        public IActionResult OnGet()
        {
            return CodeOnlyActionResult.Code404;
        }

        public IActionResult OnPost([FromForm] string reason)
        {
            if (reason != null && (reason = reason.Trim()).Length > 3)
            {
                bool isEnable = !state.AllowCreate;
                ServerStateChangedRecord record = new ServerStateChangedRecord()
                {
                    Reason = reason,
                    IsServiceEnabled = isEnable,
                    Time = DateTime.Now
                };
                query.AddServerStateChangedRecord(record);
                query.SaveChanges();
                state.RefreshState();
            }
            return RedirectToPage("Board");
        }
    }
}