using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClinicReservation.Models.Data;
using LocalizationCore;
using LocalizationCore.CodeMatching;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicReservation.Pages
{
    public class CreateModel : CultureMatchingPageModel
    {
        private readonly DataDbContext dbContext;
        private readonly ICodeMatchingService codeMatching;

        public IEnumerable<Category> Categories { get; private set; }
        public IEnumerable<Location> Locations { get; private set; }

        public CreateModel(DataDbContext dbContext, ICodeMatchingService codeMatching)
        {
            this.dbContext = dbContext;
            this.codeMatching = codeMatching;
        }

        public void OnGet()
        {
            Locations = dbContext.Locations;
            Categories = dbContext.Categories;
            codeMatching.Match(Locations);
            codeMatching.Match(Categories);
        }
    }
}