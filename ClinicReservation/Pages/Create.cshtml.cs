using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthenticationCore;
using ClinicReservation.Handlers;
using ClinicReservation.Models;
using ClinicReservation.Models.Data;
using ClinicReservation.Services;
using ClinicReservation.Services.Database;
using LocalizationCore;
using LocalizationCore.CodeMatching;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicReservation.Pages
{
    [AuthenticationRequired(AuthenticationPolicy.CASOnly, AuthenticationFailedAction.CustomHandler)]
    public class CreateModel : CultureMatchingPageModel
    {
        private readonly DataDbContext dbContext;
        private readonly IDbQuery query;
        private readonly ICodeMatchingService codeMatching;
        private readonly IAuthenticationResult authResult;
        private readonly ICultureContext cultureContext;

        public IEnumerable<Category> Categories { get; private set; }
        public IEnumerable<Location> Locations { get; private set; }
        public NewReservationFormModel LastData { get; private set; }

        public CreateModel(DataDbContext dbContext, IDbQuery query, ICodeMatchingService codeMatching, IAuthenticationResult authResult, ICultureContext cultureContext)
        {
            this.dbContext = dbContext;
            this.query = query;
            this.codeMatching = codeMatching;
            this.authResult = authResult;
            this.cultureContext = cultureContext;
        }

        [AuthenticationFailedHandler(typeof(RedirectHandler), "login")]
        public void OnGet()
        {
            Locations = dbContext.Locations;
            Categories = dbContext.Categories;
            codeMatching.Match(Locations);
            codeMatching.Match(Categories);
        }

        [AuthenticationFailedHandler(typeof(RedirectHandler), "onCreateUnauthenticated")]
        public IActionResult OnPost([FromForm] NewReservationFormModel model)
        {
            User user;
            if (ModelState.IsValid &&
               (user = query.TryGetUser(authResult.User)) != null)
            {
                DateTime now = DateTime.Now;
                Reservation reservation = new Reservation()
                {
                    Category = model.CategoryInstance,
                    CreatedDate = now,
                    Detail = model.Detail,
                    Duty = null,
                    Feedback = null,
                    LastActionDate = now,
                    LastUsedCulture = cultureContext.CurrentCulture.DisplayName,
                    LastUserModifiedDate = now,
                    Location = model.LocationInstance,
                    Poster = user,
                    ReservationDate = model.BookDateInstance,
                    State = ReservationState.Created
                };
                query.AddReservation(reservation);
                query.SaveChanges();
                int id = reservation.Id;
                return Redirect($"/detail?id={id}");
            }
            LastData = model;
            Locations = dbContext.Locations;
            Categories = dbContext.Categories;
            codeMatching.Match(Locations);
            codeMatching.Match(Categories);
            return Page();
        }
    }
}