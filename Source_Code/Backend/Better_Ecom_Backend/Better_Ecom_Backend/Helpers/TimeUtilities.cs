using Better_Ecom_Backend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Better_Ecom_Backend.Helpers
{
    public class TimeUtilities
    {
        public static int GetCurrentYear()
        {
            string currentYearString = DateTime.Today.ToString("yyyy");
            return Int32.Parse(currentYearString);
        }

        public static int GetCurrentMonth()
        {
            string currentMonthString = DateTime.Today.ToString("MM");
            return Int32.Parse(currentMonthString);
        }

        public static int GetCurrentDay()
        {
            string currentDayString = DateTime.Today.ToString("dd");
            return Int32.Parse(currentDayString);
        }

        public static DayOfWeek GetCurrentDayOfWeek()
        {
            return DateTime.Today.DayOfWeek;
        }

        public static List<Term> GetAvailableTerms()
        {
            List<Term> terms = new()
            {
                Term.First,
                Term.Second,
                Term.Summer,
                Term.Other
            };

            return terms;
        }

        public static Term GetCurrentTerm()
        {
            int currentMonth = GetCurrentMonth();
            if ((currentMonth >= 9 && currentMonth <= 12) || (currentMonth == 1))
            {
                return Term.First;
            }
            else if (currentMonth >= 2 && currentMonth <= 6)
            {
                return Term.Second;
            }
            else
            {
                return Term.Summer;
            }
        }
    }
}
