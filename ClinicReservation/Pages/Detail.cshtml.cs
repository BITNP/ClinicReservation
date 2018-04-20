using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthenticationCore;
using AuthorizationCore;
using AuthorizationCore.Attributes;
using ClinicReservation.Authorizations;
using ClinicReservation.Handlers;
using ClinicReservation.Models;
using ClinicReservation.Models.Data;
using ClinicReservation.Services.Database;
using LocalizationCore;
using LocalizationCore.CodeMatching;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicReservation.Pages
{
    [AuthenticationRequired(AuthenticationPolicy.CASOnly, AuthenticationFailedAction.CustomHandler)]
    [AuthenticationFailedHandler(typeof(RedirectHandler), "login")]
    [UserAuthorizationRequired(Policies.CanViewCurrentReservation, AuthorizationFailedAction.KeepUnauthorized)]
    public class DetailModel : CultureMatchingPageModel
    {
        private readonly IDbQuery dbQuery;
        private readonly ICodeMatchingService codeMatching;
        private readonly IAuthorizationResult result;

        public Reservation Reservation { get; private set; }

        public DetailModel(IDbQuery dbQuery, ICodeMatchingService codeMatching, IAuthorizationResult result)
        {
            this.dbQuery = dbQuery;
            this.codeMatching = codeMatching;
            this.result = result;
        }

        public IActionResult OnGet(int id)
        {
            if (!result.Succeeded())
                return CodeOnlyActionResult.Code404;

            Reservation reservation = dbQuery.TryGetReservation(id);
            dbQuery.GetDbEntry(reservation).EnsureReferencesLoaded(true);
            codeMatching.Match(reservation.Category);
            codeMatching.Match(reservation.Location);
            Reservation = reservation;
            return Page();
        }

        public IActionResult OnPost([FromForm] ReservationDetailActionFormModel model)
        {
            if (ModelState.ValidationState != ModelValidationState.Valid)
            {
                if (ModelState[nameof(model.Reservation)].ValidationState != ModelValidationState.Valid)
                    return CodeOnlyActionResult.Code404;

                Reservation reservation = model.ReservationInstance;
                dbQuery.GetDbEntry(reservation).EnsureReferencesLoaded(true);
                codeMatching.Match(reservation.Category);
                codeMatching.Match(reservation.Location);
                Reservation = reservation;
                return Page();
            }

            Reservation res = model.ReservationInstance;
            dbQuery.GetDbEntry(res).EnsureReferencesLoaded(true);
            codeMatching.Match(res.Category);
            codeMatching.Match(res.Location);
            Reservation = res;
            switch (model.Action)
            {
                case "message":
                    break;
                case "cancel":
                    break;
                case "accept":
                    break;
                case "feedback":
                    break;
                case "":
                    break;
                default:
                    return CodeOnlyActionResult.Code404;
            }

            return Page();
        }
    }
}