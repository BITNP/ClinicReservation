using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClinicReservation.Helpers
{
    public static class DateTimeHelper
    {
        public static DateTime GetBeijingTime()
        {
            return DateTime.UtcNow.AddHours(8);
        }
    }
}
