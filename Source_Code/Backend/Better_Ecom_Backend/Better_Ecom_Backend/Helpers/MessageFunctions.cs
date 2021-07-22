﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Better_Ecom_Backend.Helpers
{
    public class MessageFunctions
    {
        public static string GetNotImplementedString()
        {
            return "not implemented yet.";
        }

        public static string GetMaybeDatabaseIsDownMessage()
        {
            return "unknown error, maybe database is down.";
        }

        public static string GetRequiredDataMissingOrInvalidMessage()
        {
            return "required data missing or invalid.";
        }

        public static string GetSystemUserNotFoundMessage()
        {
            return "no user was found with the provided id.";
        }

        public static string GetStudentNotFoundMessage()
        {
            return "no student was found with the provided id.";
        }

        public static string GetInstructorNotFoundMessage()
        {
            return "no instructor was found with the provided id.";
        }

        public static string GetAdminNotFoundMessage()
        {
            return "no admin was found with the provided id.";
        }

        public static string GetCourseNotFoundMessage()
        {
            return "no course was found with the provided code.";
        }

        public static string GetCourseInstanceNotFoundMessage()
        {
            return "no course instance was found with the provided id.";
        }
    }
}