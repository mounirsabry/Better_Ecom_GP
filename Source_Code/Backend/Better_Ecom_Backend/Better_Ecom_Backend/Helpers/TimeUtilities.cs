﻿using Better_Ecom_Backend.Models;
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
        /*
        public static Term GetCurrentTerm()
        {

        }

        public static int GetCurrentMonth()
        {

        }

        public static int GetCurrentDay()
        {

        }
        */
    }
}