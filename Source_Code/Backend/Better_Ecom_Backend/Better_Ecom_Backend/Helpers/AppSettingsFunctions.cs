﻿using Microsoft.Extensions.Configuration;
using System;

namespace Better_Ecom_Backend.Helpers
{
    public class AppSettingsFunctions
    {
        public static bool GetIsCourseRegistrationOpen(IConfiguration _config)
        {
            return Convert.ToBoolean(_config["CourseRegistrationSettings:IsCourseRegistrationOpen"]);
        }

        public static bool GetIsLateCourseRegistrationOpen(IConfiguration _config)
        {
            return Convert.ToBoolean(_config["CourseRegistrationSettings:IsLateCourseRegistrationOpen"]);
        }

        public static bool GetIsDropCourseRegistrationOpen(IConfiguration _config)
        {
            return Convert.ToBoolean(_config["CourseRegistrationSettings:IsDropCourseRegistrationOpen"]);
        }
    }
}
