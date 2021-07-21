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

        [HttpGet("GetStudentAvailableCourses/{UserID:int}")]
        public IActionResult GetStudentAvailableCourses(int userID)
        {
            //STUDENT, ADMIN FUNCTION.
            return Ok(new { Message = HelperFunctions.GetNotImplementedString() });
        }

        [HttpGet("GetCourseAvailableCourseInstances/{CourseCode}")]
        public IActionResult GetCourseAvailableCourseInstances(string courseCode)
        {
            //STUDENT, ADMIN FUNCTION.
            return Ok(new { Message = HelperFunctions.GetNotImplementedString() });
        }

        [HttpGet("GetStudentRegisteredCourseInstances/{StudentID:int}")]
        public IActionResult GetStudentRegisteredCourseInstances(int studentID)
        {
            //STUDENT, ADMIN FUNCTION.

            //Check the token, if the user is an admin, then accept any valid student ID.
            //If the user is a student, then the sent student ID must match the id in the token.
            return Ok(new { Message = HelperFunctions.GetNotImplementedString() });
        }

        [HttpPost("RegisterToCourseInstance")]
        public IActionResult RegisterToCourseInstance([FromBody] JsonElement jsonInput)
        {
            //STUDENT, ADMIN FUNCTION.
            return Ok(new { Message = HelperFunctions.GetNotImplementedString() });
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

        private static bool IsFromStudentAvailableCoursesList(int userID)
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
    }
}
