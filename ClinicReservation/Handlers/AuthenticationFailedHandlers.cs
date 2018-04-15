using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ClinicReservation.Handlers
{
    public class RedirectHandler
    {
        private readonly string target;
        public RedirectHandler(string target)
        {
            this.target = target;
        }

        public IActionResult Invoke()
        {
            return new RedirectToPageResult(target);
        }
    }

    public class CustomReturnCodeHandler
    {
        

        private readonly int code;
        public CustomReturnCodeHandler(int code)
        {
            this.code = code;
        }

        public IActionResult Invoke()
        {
            return new CodeOnlyActionResult(code);
        }
    }
}
