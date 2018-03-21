using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicReservation.Pages
{
    public class RegionModel : PageModel
    {
        public string Source { get; private set; }

        public void OnGet(string source = null)
        {
            if (source == null)
                Source = "";
            else if (source.StartsWith('/'))
                Source = source.Substring(1);
            else
                Source = source;
        }
    }
}