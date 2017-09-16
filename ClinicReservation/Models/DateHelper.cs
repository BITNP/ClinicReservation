using ClinicReservation.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClinicReservation.Models
{
    public static class DateHelper
    {
        private static Dictionary<DayOfWeek, string> DAY_NAME = new Dictionary<DayOfWeek, string>
        {
            [DayOfWeek.Monday] = "周一",
            [DayOfWeek.Tuesday] = "周二",
            [DayOfWeek.Wednesday] = "周三",
            [DayOfWeek.Thursday] = "周四",
            [DayOfWeek.Friday] = "周五",
            [DayOfWeek.Saturday] = "周六",
            [DayOfWeek.Sunday] = "周日"
        };
        private static string NEXT_WEEK_PREFIX = "下";


        public static string[] GetDateStrings()
        {
            string[] datenames = new string[7];
            datenames[0] = "今天";
            datenames[1] = "明天";
            DateTime now = DateTimeHelper.GetBeijingTime();
            DayOfWeek cdow = now.DayOfWeek, tdow;
            for (int i = 2; i < datenames.Length; i++)
            {
                tdow = now.AddDays(i).DayOfWeek;
                if (tdow < cdow && tdow != DayOfWeek.Sunday)
                    datenames[i] = NEXT_WEEK_PREFIX + DAY_NAME[tdow];
                else
                    datenames[i] = DAY_NAME[tdow];
            }

            return datenames;
        }
    }
}
