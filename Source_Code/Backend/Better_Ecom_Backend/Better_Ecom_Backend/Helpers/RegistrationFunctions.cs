using DataLibrary;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Better_Ecom_Backend.Helpers
{
    public class RegistrationFunctions
    {
        public static bool IsStudentRegisteredToCourse(IConfiguration _config, IDataAccess _data, int studentID, string courseCode)
        {
            return true;
        }

        public static bool IsStudentRegisteredToCourseInstance(IConfiguration _config, IDataAccess _data, int studentID, int courseInstanceID)
        {
            string getInstructorIDFromCourseInstance = "SELECT student_id FROM student_course_instance_registration" +
                               " WHERE student_id = @StudentID AND course_instance_id = @CourseInstanceID";

            List<bool> ids = _data.LoadData<bool, dynamic>(getInstructorIDFromCourseInstance, new { StudentID = studentID, CourseInstanceID = courseInstanceID }, _config.GetConnectionString("Default"));

            if (ids is null || ids.Count == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static bool IsInstructorRegisteredToCourse(IConfiguration _config, IDataAccess _data, int instructorID, string courseCode)
        {
            return true;
        }

        public static bool IsInstructorRegisteredToCourseInstance(IConfiguration _config, IDataAccess _data, int instructorID, int courseInstanceID)
        {
            string getInstructorIDFromCourseInstance = "SELECT instructor_id FROM instructor_course_instance_registration" +
                                           " WHERE instructor_id = @InstructorID AND course_instance_id = @CourseInstanceID";

            List<bool> ids = _data.LoadData<bool, dynamic>(getInstructorIDFromCourseInstance, new { InstructorID = instructorID, CourseInstanceID = courseInstanceID }, _config.GetConnectionString("Default"));

            if (ids is null || ids.Count == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
