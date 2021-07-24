using Better_Ecom_Backend.Entities;
using Better_Ecom_Backend.Helpers;
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
    public class GradeController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IDataAccess _data;

        public GradeController(IConfiguration config, IDataAccess data)
        {
            _config = config;
            _data = data;
        }

        [Authorize]
        [HttpGet("GetStudentCourseInstanceGrade/{StudentID:int}/{CourseInstanceID:int}")]
        public IActionResult GetStudentCourseInstanceGrade([FromHeader] string Authorization, int studentID, int courseInstanceID)
        {
            TokenInfo tokenInfo = HelperFunctions.GetIdAndTypeFromToken(Authorization);
            if (tokenInfo.Type == "student" && tokenInfo.UserID != studentID)
            {
                return Forbid("students can only get their own data.");
            }

            if (tokenInfo.Type == "instructor")
            {
                if (RegistrationFunctions.IsInstructorRegisteredToCourseInstance(_config, _data, tokenInfo.UserID, courseInstanceID) == false)
                {
                    return Forbid("instructors can only view the data of the courses they are registered to.");
                }
            }

            else if (ExistanceFunctions.IsStudentExists(_config, _data, studentID) == false)
            {
                return BadRequest(new { Message = MessageFunctions.GetStudentNotFoundMessage() });
            }
            else if (ExistanceFunctions.IsCourseInstanceExists(_config, _data, courseInstanceID) == false)
            {
                return BadRequest(new { Message = MessageFunctions.GetCourseInstanceNotFoundMessage() });
            }
            else if (RegistrationFunctions.IsStudentRegisteredToCourseInstance(_config, _data, studentID, courseInstanceID) == false)
            {
                return BadRequest(new { Message = MessageFunctions.GetStudentNotRegisteredToCourseInstanceMessage() });
            }

            string sql = "SELECT student_course_instance_grade FROM student_course_instance_registration" + "\n"
                       + "WHERE student_id = @StudentID AND course_instance_id = @CourseInstanceID;";

            List<string> gradeList = _data.LoadData<string, dynamic>(sql, new { StudentID = studentID, CourseInstanceID = courseInstanceID }, 
                _config.GetConnectionString("Default"));

            if (gradeList is null || gradeList.Count == 0)
            {
                return BadRequest(new { Message = MessageFunctions.GetMaybeDatabaseIsDownMessage() });
            }
            return Ok(gradeList[0]);
        }

        [Authorize(Roles = "admin")]
        [HttpPatch("SetStudentCourseInstanceGrade")]
        public IActionResult SetStudentCourseInstanceGrade([FromBody] JsonElement jsonInput)
        {
            if (IsSetStudentCourseInstanceGradeDataValid(jsonInput) == false)
            {
                return BadRequest(new { Message = MessageFunctions.GetRequiredDataMissingOrInvalidMessage() });
            }
            int studentID = jsonInput.GetProperty("StudentID").GetInt32();
            int courseInstanceID = jsonInput.GetProperty("CourseInstanceID").GetInt32();
            StudentCourseInstanceGrade grade = (StudentCourseInstanceGrade)jsonInput.GetProperty("Grade").GetInt32();

            if (ExistanceFunctions.IsStudentExists(_config, _data, studentID) == false)
            {
                return BadRequest(new { Message = MessageFunctions.GetStudentNotFoundMessage() });
            }
            else if (ExistanceFunctions.IsCourseInstanceExists(_config, _data, courseInstanceID) == false)
            {
                return BadRequest(new { Message = MessageFunctions.GetCourseInstanceNotFoundMessage() });
            }
            else if (RegistrationFunctions.IsStudentRegisteredToCourseInstance(_config, _data, studentID, courseInstanceID) == false)
            {
                return BadRequest(new { Message = MessageFunctions.GetStudentNotRegisteredToCourseInstanceMessage() });
            }

            string sql = "UPDATE student_course_instance_registration" + "\n"
                       + "SET student_course_instance_grade = @Grade" + "\n"
                       + "WHERE student_id = @StudentID" + "\n"
                       + "AND course_instance_id = @CourseInstanceID;";
            var parameters = new
            {
                StudentID = studentID,
                CourseInstanceID = courseInstanceID,
                Grade = grade
            };

            int state = _data.SaveData<dynamic>(sql, parameters, _config.GetConnectionString("Default"));
            if (state < 0)
            {
                return BadRequest(new { Message = MessageFunctions.GetMaybeDatabaseIsDownMessage() });
            }

            if (RecalculateStudentGPA(studentID) == false)
            {
                return BadRequest(new { Message = "server problem, or gpa recalculation failed." });
            }
            return Ok(new { Message = "grade was set successfully." });
        }

        [Authorize(Roles = "admin, student")]
        [HttpGet("GetStudentGPA/{StudentID}")]
        public IActionResult GetStudentGPA([FromHeader] string Authorization, int studentID)
        {
            TokenInfo tokenInfo = HelperFunctions.GetIdAndTypeFromToken(Authorization);
            if (tokenInfo.Type == "student" && tokenInfo.UserID != studentID)
            {
                return Forbid("students can only get their own data.");
            }

            if (ExistanceFunctions.IsStudentExists(_config, _data, studentID) == false)
            {
                return BadRequest(new { Message = MessageFunctions.GetStudentNotFoundMessage() });
            }

            string sql = "SELECT gpa FROM student WHERE student_id = @StudentID;";
            List<double> gpaList = _data.LoadData<double, dynamic>(sql, new { StudentID = studentID }, _config.GetConnectionString("Default"));

            if (gpaList is null || gpaList.Count == 0)
            {
                return BadRequest(new { Message = MessageFunctions.GetMaybeDatabaseIsDownMessage() });
            }

            return Ok(gpaList[0]);
        }

        private static bool IsSetStudentCourseInstanceGradeDataValid(JsonElement jsonInput)
        {
            if (jsonInput.TryGetProperty("StudentID", out JsonElement temp) && temp.TryGetInt32(out _)
            && jsonInput.TryGetProperty("CourseInstanceID", out temp) && temp.TryGetInt32(out _)
            && jsonInput.TryGetProperty("Grade", out temp) && temp.TryGetInt32(out Int32 gradeInt)
            && Enum.IsDefined((StudentCourseInstanceGrade)gradeInt))
            {
                return true;
            }
            return false;
        }

        private static bool RecalculateStudentGPA(int studentID)
        {
            /*
            Return false in case the operation failed.
            Otherwise return true.
            */
            return true;
        }
    }
}
