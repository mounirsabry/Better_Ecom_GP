using DataLibrary;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Better_Ecom_Backend.Helpers
{
    public class ExistanceFunctions
    {
        public static bool IsSystemUserExists(IConfiguration _config, IDataAccess _data, int userID)
        {
            return true;
        }

        public static bool IsStudentExists(IConfiguration _config, IDataAccess _data, int studentID)
        {
            return true;
        }

        public static bool IsInstructorExists(IConfiguration _config, IDataAccess _data, int instructorID)
        {
            return true;
        }

        public static bool IsAdminExists(IConfiguration _config, IDataAccess _data, int adminID)
        {
            return true;
        }

        public static bool IsCourseExists(IConfiguration _config, IDataAccess _data, string courseCode)
        {
            return true;
        }

        public static bool IsCourseInstanceExists(IConfiguration _config, IDataAccess _data, int courseInstanceID)
        {
            return true;
        }
    }
}
