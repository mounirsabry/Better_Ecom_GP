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
        public static bool IsDBUpAndRunning(IConfiguration _config, IDataAccess _data)
        {
            string sql = "SELECT max(system_user_id) FROM system_user;";
            List<int> IDs = _data.LoadData<int, dynamic>(sql, new { }, _config.GetConnectionString("Default"));
            if (IDs is null)
            {
                return false;
            }
            return true;
        }

        public static bool IsSystemUserExists(IConfiguration _config, IDataAccess _data, int userID)
        {
            string sql = "SELECT system_user_id FROM system_user WHERE system_user_id = @UserID;";
            List<int> users = _data.LoadData<int, dynamic>(sql, new { UserID = userID }, _config.GetConnectionString("Default"));
            if (users is null || users.Count == 0)
            {
                return false;
            }
            return true;
        }

        public static bool IsStudentExists(IConfiguration _config, IDataAccess _data, int studentID)
        {
            string sql = "SELECT student_id FROM student WHERE student_id = @StudentID;";
            List<int> students = _data.LoadData<int, dynamic>(sql, new { StudentID = studentID }, _config.GetConnectionString("Default"));
            if (students is null || students.Count == 0)
            {
                return false;
            }
            return true;
        }

        public static bool IsInstructorExists(IConfiguration _config, IDataAccess _data, int instructorID)
        {
            string sql = "SELECT instructor_id FROM instructor WHERE instructor_id = @InstructorID;";
            List<int> instructors = _data.LoadData<int, dynamic>(sql, new { InstructorID = instructorID }, _config.GetConnectionString("Default"));
            if (instructors is null || instructors.Count == 0)
            {
                return false;
            }
            return true;
        }

        public static bool IsAdminExists(IConfiguration _config, IDataAccess _data, int adminID)
        {
            string sql = "SELECT admin_user_id FROM admin_user WHERE admin_user_id = @AdminID;";
            List<int> admins = _data.LoadData<int, dynamic>(sql, new { AdminID = adminID }, _config.GetConnectionString("Default"));
            if (admins is null || admins.Count == 0)
            {
                return false;
            }
            return true;
        }

        public static bool IsCourseExists(IConfiguration _config, IDataAccess _data, string courseCode)
        {
            string sql = "SELECT course_code FROM course WHERE course_code = @CourseCode;";
            List<string> courseCodes = _data.LoadData<string, dynamic>(sql, new { CourseCode = courseCode }, _config.GetConnectionString("Default"));
            if (courseCodes is null || courseCodes.Count == 0)
            {
                return false;
            }
            return true;
        }

        public static bool IsCourseInstanceExists(IConfiguration _config, IDataAccess _data, int courseInstanceID)
        {
            string sql = "SELECT instance_id from course_instance WHERE instance_id = @InstanceID;";
            List<int> instancesIDs = _data.LoadData<int, dynamic>(sql, new { InstanceID = courseInstanceID }, _config.GetConnectionString("Default"));
            if (instancesIDs is null || instancesIDs.Count == 0)
            {
                return false;
            }
            return true;
        }

        public static bool IsAttendanceItemExists(IConfiguration _config, IDataAccess _data, int attendanceItemID)
        {
            return true;
        }
    }
}
