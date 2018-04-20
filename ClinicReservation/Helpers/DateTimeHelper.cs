using LocalizationCore.CodeMatching;
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
        public static DateTime NowDate()
        {
            DateTime now = DateTime.Now;
            return new DateTime(year: now.Year, month: now.Month, day: now.Day);
        }

        public static string[] GetDisplayDateTime(DateTime currentDate, int count, ICodeMatchingService matchingService)
        {
            bool isNextWeek = false;
            string[] result = new string[count];
            DayOfWeek dayOfWeek = currentDate.DayOfWeek;
            string name = "day_" + dayOfWeek.ToString();
            TimeSpan oneDay = TimeSpan.FromDays(1);
            currentDate += oneDay;
            count--;
            int index = 1;
            string nextPrefix = "";
            while (count > 0)
            {
                dayOfWeek = currentDate.DayOfWeek;
                name = "day_" + dayOfWeek.ToString();

                if (dayOfWeek == DayOfWeek.Monday)
                {
                    nextPrefix = matchingService.Match("day_Next", "Next {0}");
                    isNextWeek = true;
                }
                if (isNextWeek)
                {
                    result[index] = string.Format(nextPrefix, matchingService.Match(name, dayOfWeek.ToString()));
                }
                else
                {
                    result[index] = matchingService.Match(name, dayOfWeek.ToString());
                }
                currentDate += oneDay;
                count--;
                index++;

            }
            result[0] = matchingService.Match("day_Today", "Today");
            result[1] = matchingService.Match("day_Tomorrow", "Tomorrow");
            return result;
        }
    }
}
