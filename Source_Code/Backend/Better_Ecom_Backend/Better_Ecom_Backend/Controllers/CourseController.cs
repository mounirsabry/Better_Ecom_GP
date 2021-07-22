using Better_Ecom_Backend.Helpers;
using Better_Ecom_Backend.Models;
using DataLibrary;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Better_Ecom_Backend.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IDataAccess _data;

        public CourseController(IConfiguration config, IDataAccess data)
        {
            _config = config;
            _data = data;
        }

        [Authorize(Roles = "student, admin")]
        [HttpGet("GetStudentAvailableCourses/{StudentID:int}")]
        public IActionResult GetStudentAvailableCourses([FromHeader] string Authorization, int StudentID)
        {
            //STUDENT, ADMIN FUNCTION.
            TokenInfo info = HelperFunctions.GetIdAndTypeFromToken(Authorization);
            if (info.Type == "student" && info.UserID != StudentID)
            {
                return Forbid("students can only get their own data.");
            }

            return Ok(new { Message = HelperFunctions.GetNotImplementedString() });
        }

        [HttpGet("GetCourseAvailableCourseInstances/{CourseCode}")]
        public IActionResult GetCourseAvailableCourseInstances([FromHeader] string Authorization, string courseCode)
        {
            //STUDENT, ADMIN FUNCTION.

            return Ok(new { Message = HelperFunctions.GetNotImplementedString() });
        }

        [Authorize(Roles = "admin, student")]
        [HttpGet("GetStudentRegisteredCourses/{StudentID:int")]
        public IActionResult GetStudentRegisteredCourses([FromHeader] string Authorization, int studentID)
        {
            TokenInfo info = HelperFunctions.GetIdAndTypeFromToken(Authorization);
            if (info.Type == "student" && info.UserID != studentID)
            {
                return Forbid("students can only get their own data.");
            }
            return Ok(new { Message = HelperFunctions.GetNotImplementedString() });
        }

        [Authorize(Roles = "admin, student")]
        [HttpGet("GetStudentRegisteredCourseInstances/{StudentID:int}")]
        public IActionResult GetStudentRegisteredCourseInstances([FromHeader] string Authorization, int studentID)
        {
            //STUDENT, ADMIN FUNCTION.
            TokenInfo info = HelperFunctions.GetIdAndTypeFromToken(Authorization);
            if (info.Type == "student" && info.UserID != studentID)
            {
                return Forbid("students can only get their own data.");
            }

            string getCourseInstanceByStudentSql = "SELECT course_instance_id FROM student_course_instance_registration WHERE student_id = @studentID";
            string getStudentRegisteredCourseInstancesSql = $"SELECT * FROM course_instance WHERE instance_id in ({getCourseInstanceByStudentSql}); ";

            List<Course_instance> instances = _data.LoadData<Course_instance, dynamic>(getStudentRegisteredCourseInstancesSql, new { studentID }, _config.GetConnectionString("Default"));

            if (instances is null)
            {
                return BadRequest(new { Message = "unknown error, maybe database server is down." });
            }

            return Ok(instances);
        }

        [Authorize(Roles = "admin, student")]
        [HttpGet("GetCourseStudentRegisteredCourseInstances/{StudentID:int}/{CourseCode}")]
        public IActionResult GetCourseStudentRegisteredCourseInstances([FromHeader] string Authorization, int studentID, string courseCode)
        {
            //STUDENT, ADMIN FUNCTION.
            TokenInfo info = HelperFunctions.GetIdAndTypeFromToken(Authorization);
            if (info.Type == "student" && info.UserID != studentID)
            {
                return Forbid("students can only get their own data.");
            }

            return Ok(new { Message = HelperFunctions.GetNotImplementedString() });
        }

        [Authorize(Roles = "student, admin")]
        [HttpPost("RegisterToCourseInstance")]
        public IActionResult RegisterToCourseInstance([FromHeader] string Authorization, [FromBody] JsonElement jsonInput)
        {
            //STUDENT, ADMIN FUNCTION.
            if (AppSettingsFunctions.GetIsCourseRegistrationOpen(_config) == false)
            {
                return BadRequest(new { Message = "normal course registration is unavailable, try check late course registration." });
            }

            if (!RegisterToCourseInstanceDataValid(jsonInput))
            {
                return BadRequest(new { Message = "required data missing or invalid." });
            }

            int studentID = jsonInput.GetProperty("StudentID").GetInt32();
            int courseInstanceID = jsonInput.GetProperty("CourseInstanceID").GetInt32();

            TokenInfo info = HelperFunctions.GetIdAndTypeFromToken(Authorization);
            if (info.Type == "student" && info.UserID != studentID)
            {
                return Forbid("students can only get their own data.");
            }

            string courseCode;
            //We still have to check that the course is valid for the user.
            List<string> courseCodes = GetCourseCodesListFromCourseInstanceID(courseInstanceID);
            if (courseCodes is null)
            {
                return BadRequest(new { Message = HelperFunctions.GetMaybeDatabaseIsDownMessage() });
            }
            else
            {
                courseCode = courseCodes[0];
            }

            if (IsFromStudentAvailableCoursesList(studentID, courseCode) == false)
            {
                return BadRequest("this course is unavailable for the student.");
            }

            if (IsFromCourseAvailableCourseInstancesList(courseInstanceID) == false)
            {
                return BadRequest("this course instance is unavailable for registration.");
            }

            DateTime registrationDate = DateTime.Now;
            Student_course_instance_registration registration = new(-1, studentID, courseInstanceID, registrationDate, StudentCourseInstanceRegistrationStatus.Undertaking);

            string insertCourseRegistrationSql = "INSERT INTO student_course_instance_registration VALUES(NULL, @student_id, @course_instance_id, @registration_date, @student_course_intance_status);";

            int status = _data.SaveData(insertCourseRegistrationSql, registration, _config.GetConnectionString("Default"));

            if (status > 0)
            {
                return Ok(registration);
            }
            else
            {
                return BadRequest(new { Message = "unknown error, maybe database server is down." });
            }
        }

        [HttpDelete("DropStudentFromCourseInstance")]
        public IActionResult DropStudentFromCourseInstance([FromBody] JsonElement jsonInput)
        {
            //STUDENT, ADMIN FUNCTION.
            return Ok(new { Message = HelperFunctions.GetNotImplementedString() });
        }

        [Authorize]
        [HttpGet("GetCourseInstanceRegisteredStudents/{CourseInstanceID:int}")]
        public IActionResult GetCourseInstanceRegisteredStudents(int courseInstanceID)
        {
            //ALL UESRS.
            return Ok(new { Message = HelperFunctions.GetNotImplementedString() });
        }

        [HttpGet("GetAllLateCourseInstanceRegistrationRequests")]
        public IActionResult GetAllLateCourseInstanceRegistrationRequests()
        {
            //ADMIN ONLY FUNCTION.
            return Ok(new { Message = HelperFunctions.GetNotImplementedString() });
        }

        [HttpGet("GetCourseLateCourseRegistrationRequests/{CourseCode}")]
        public IActionResult GetCourseLateCourseRegistrationRequests(string courseCode)
        {
            //ADMIN ONLY FUNCTION.
            return Ok(new { Message = HelperFunctions.GetNotImplementedString() });
        }

        [HttpGet("GetStudentLateCourseInstanceRegistrationRequests/{StudentID:int}")]
        public IActionResult GetStudentLateCourseInstanceRegistrationRequests(int studentID)
        {
            //STUDENT, ADMIN FUNCTION.
            return Ok(new { Message = HelperFunctions.GetNotImplementedString() });
        }

        [HttpGet("GetLateCourseInstanceRegistrationRequestAvailableStatus")]
        public IActionResult GetLateCourseInstanceRegistrationRequestAvailableStauts()
        {
            //STUDENT, ADMIN FUNCTION.
            return Ok(EnumFunctions.GetLateRegistrationRequestStausList());
        }

        [HttpPost("SubmitLateCourseInstanceRegistrationRequest")]
        public IActionResult SubmitLateCourseInstanceRegistrationRequest([FromBody] JsonElement jsonInput)
        {
            //STUDENT ONLY FUNCTION.
            return Ok(new { Message = HelperFunctions.GetNotImplementedString() });
        }

        [HttpDelete("DeleteLateCourseInstanceRegistrationRequest")]
        public IActionResult DeleteLateCourseInstanceRegistrationRequest([FromBody] JsonElement jsonInput)
        {
            //STUDENT ONLY FUNCTION.
            return Ok(new { Message = HelperFunctions.GetNotImplementedString() });
        }

        [HttpPatch("SetLateCourseInstanceRegistrationRequest")]
        public IActionResult SetLateCourseInstanceRegistrationRequest([FromBody] JsonElement jsonInput)
        {
            //ADMIN ONLY FUNCTION.
            return Ok(new { Message = HelperFunctions.GetNotImplementedString() });
        }

        [Authorize]
        [HttpGet("GetStudentCourseInstanceRegistrationAvailableStatus")]
        public IActionResult GetStudentCourseInstanceRegistrationAvailableStatus()
        {
            //ALL USERS.
            return Ok(EnumFunctions.GetStudentCourseInstanceRegistrationStatusList());
        }

        [Authorize]
        [HttpGet("GetStudentCourseInstanceStatus/{StudentID:int}/{CourseInstanceID:int}")]
        public IActionResult GetStudentCourseInstanceStatus(int studentID, int courseInstanceID)
        {
            //ALL USERS.
            return Ok(new { Message = HelperFunctions.GetNotImplementedString() });
        }

        [HttpPatch("SetStudentCourseInstanceStatus")]
        public IActionResult SetStudentCourseInstanceStatus([FromBody] JsonElement jsonInput)
        {
            //ADMIN ONLY FUNCTION.
            return Ok(new { Message = HelperFunctions.GetNotImplementedString() });
        }

        private List<Course> GetStudentAvailableCoursesList(int studentID)
        {
            return null;
        }

        private List<Course_instance> GetCourseAvailableCourseInstancesList(string courseCode)
        {
            //Take the course code, return all the available course instances for registration.
            /*
            The query is based on the course code, current working year, current term, and is the course instance
            is close for registration or not.
            */
            int workingYear = TimeUtilities.GetCurrentYear();
            int currentMonth = TimeUtilities.GetCurrentMonth();
            Term currentTerm = TimeUtilities.GetCurrentTerm();
            if (currentMonth == 1 && currentTerm == Term.First)
            {
                workingYear -= 1;
            }

            string sql = "SELECT * FROM course_instance" + "\n"
                    + "WHERE course_code = @Coursecode" + "\n"
                    + "AND course_year = @WorkingYear" + "\n"
                    + "AND course_term = @CurrentTerm" + "\n"
                    + "OR course_term = 'Other'" + "\n"
                    + "AND is_closed_for_registration = FALSE;";


            return null;
        }

        private static bool IsFromStudentAvailableCoursesList(int userID, string courseCode)
        {
            return true;
        }

        private static bool IsFromCourseAvailableCourseInstancesList(int courseInstanceID)
        {
            return true;
        }

        private List<string> GetCourseCodesListFromCourseInstanceID(int courseInstanceID)
        {
            string sql = "SELECT course_code FROM course" + "\n"
                + "INNER JOIN course_instance" + "\n"
                + "WHERE course.course_code = course_instance.course_code" + "\n"
                + "AND instance_id = @courseInstanceID;";

            List<string> courseCodes = _data.LoadData<string, dynamic>(sql, new { courseInstanceID }, _config.GetConnectionString("Default"));
            if (courseCodes is null)
            {
                return null;
            }
            return courseCodes;
        }

        private bool RegisterToCourseInstanceDataValid(JsonElement jsonInput)
        {
            throw new NotImplementedException();
        }
    }
}
