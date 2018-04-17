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
    [AuthenticationFailedHandler(typeof(RedirectHandler), "/login")]
    [UserAuthorizationRequired(Policies.CanModifyCurrentReservation, AuthorizationFailedAction.KeepUnauthorized)]
    public class ModifyModel : CultureMatchingPageModel
    {
        private readonly DataDbContext dbContext;
        private readonly IDbQuery dbQuery;
        private readonly ICodeMatchingService matching;
        private readonly IAuthorizationResult result;

        public ModifyReservationFormModel Reservation { get; private set; }
        public IEnumerable<Category> Categories { get; private set; }
        public IEnumerable<Location> Locations { get; private set; }
        public bool CheckDetailError { get; private set; }

        public ModifyModel(DataDbContext dbContext, IDbQuery dbQuery, ICodeMatchingService matching, IAuthorizationResult result)
        {
            this.dbContext = dbContext;
            this.dbQuery = dbQuery;
            this.matching = matching;
            this.result = result;
        }

        public IActionResult OnGet(int id)
        {
            if (!result.Succeeded())
                return CodeOnlyActionResult.Code404;

            // TODO: chech the state of the current reservation

            Reservation reservation = dbQuery.TryGetReservation(id);
            dbQuery.GetDbEntry(reservation).EnsureReferencesLoaded(true);
            Reservation = new ModifyReservationFormModel(reservation);
            Locations = dbContext.Locations;
            Categories = dbContext.Categories;
            matching.Match(Locations);
            matching.Match(Categories);
            CheckDetailError = false;
            return Page();
        }

        public IActionResult OnPost(ModifyReservationFormModel form)
        {
            if (!result.Succeeded())
                return CodeOnlyActionResult.Code401;

            // TODO: chech the state of the reservation from form.ReservationInstance

            if (ModelState.IsValid)
            {
                Reservation reservation = form.ReservationInstance;
                reservation.Category = form.CategoryInstance;
                reservation.Detail = form.Detail;
                reservation.Location = form.LocationInstance;
                reservation.ReservationDate = form.BookDateInstance;
                // TODO: save modification to database
            }
            if (ModelState[nameof(form.Reservation)].ValidationState == ModelValidationState.Valid)
            {
                dbQuery.GetDbEntry(form.ReservationInstance).EnsureReferencesLoaded(true);
                Reservation = form;
                Locations = dbContext.Locations;
                Categories = dbContext.Categories;
                matching.Match(Locations);
                matching.Match(Categories);
                CheckDetailError = true;
                return Page();
            }
            else
            {
                return CodeOnlyActionResult.Code404;
            }

        }
    }
}