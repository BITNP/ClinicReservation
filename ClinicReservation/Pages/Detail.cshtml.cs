using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthenticationCore;
using ClinicReservation.Handlers;
using ClinicReservation.Models;
using ClinicReservation.Models.Data;
using ClinicReservation.Services.Database;
using LocalizationCore;
using LocalizationCore.CodeMatching;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicReservation.Pages
{
    [AuthenticationRequired(failedAction: AuthenticationFailedAction.CustomHandler)]
    public class DetailModel : CultureMatchingPageModel
    {
        private readonly IDbQuery dbQuery;
        private readonly IAuthenticationResult authResult;
        private readonly ICodeMatchingService codeMatching;

        public Reservation Reservation { get; private set; }

        public DetailModel(IDbQuery dbQuery, IAuthenticationResult authResult, ICodeMatchingService codeMatching)
        {
            this.dbQuery = dbQuery;
            this.authResult = authResult;
            this.codeMatching = codeMatching;
        }

        [CustomHandler(typeof(RedirectHandler), "login")]
        public IActionResult OnGet(int id)
        {
            Reservation reservation = dbQuery.TryGetReservation(id);
            User user = dbQuery.TryGetUser(authResult.User);
            if (reservation == null || user == null)
            {
                // redirect 404
                return Forbid();
            }

            // check if the current user is the owner of this reservation
            // or if the current user is a duty memeber (to be added)
            if (reservation.Poster != user)
            {
                return Forbid();
            }
            dbQuery.GetDbEntry(reservation).EnsureReferencesLoaded(true);
            codeMatching.Match(reservation.Category);
            codeMatching.Match(reservation.Location);
            Reservation = reservation;
            return Page();
        }
    }
}