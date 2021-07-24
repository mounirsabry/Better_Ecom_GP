using Better_Ecom_Backend.Entities;
using System;
using System.Collections.Generic;

namespace Better_Ecom_Backend.Helpers
{
    public class TimeUtilities
    {
        public static int GetCurrentYear()
        {
            string currentYearString = DateTime.Today.ToString("yyyy");
            return Int32.Parse(currentYearString);
        }

        public static int GetYearFromDate(DateTime date)
        {
            string dateYearString = date.ToString("yyyy");
            return Int32.Parse(dateYearString);
        }

        public static int GetCurrentMonth()
        {
            string currentMonthString = DateTime.Today.ToString("MM");
            return Int32.Parse(currentMonthString);
        }

        public static int GetMonthFromDate(DateTime date)
        {
            string dateMonthString = date.ToString("MM");
            return Int32.Parse(dateMonthString);
        }

        public static int GetCurrentDay()
        {
            string currentDayString = DateTime.Today.ToString("dd");
            return Int32.Parse(currentDayString);
        }

        public static int GetDayFromDate(DateTime date)
        {
            string dateDayString = date.ToString("dd");
            return Int32.Parse(dateDayString);
        }

        public static DayOfWeek GetCurrentDayOfWeek()
        {
            return DateTime.Today.DayOfWeek;
        }

        public static DayOfWeek GetDayOfWeekFromDate(DateTime date)
        {
            return date.DayOfWeek;
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

        public static Term GetTermFromMonth(int month)
        {
            if ((month >= 9 && month <= 12) || (month == 1))
            {
                return Term.First;
            }
            else if (month >= 2 && month <= 6)
            {
                return Term.Second;
            }
            else
            {
                return Term.Summer;
            }
        }

        public static Term GetCurrentTerm()
        {
            int currentMonth = GetCurrentMonth();
            return GetTermFromMonth(currentMonth);
        }

        public static Term GetTermFromDate(DateTime date)
        {
            int dateMonth = GetMonthFromDate(date);
            return GetTermFromMonth(dateMonth);
        }
    }
}
