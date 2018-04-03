using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClinicReservation
{
    public class RedirectHandler
    {
        private string target;
        public RedirectHandler(string target)
        {
            this.target = target;
        }

        public IActionResult Invoke()
        {
            return new RedirectToPageResult(target);
        }
    }
}
