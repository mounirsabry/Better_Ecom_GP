using Better_Ecom_Backend.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Better_Ecom_Backend.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AttendanceController : ControllerBase
    {
        [Authorize]
        [HttpGet("GetStudentCourseInstanceAttendance/{CourseInstanceID:int}/{StudentID:int}")]
        public IActionResult GetStudentCourseInstanceAttendance([FromHeader] string Authroization, int courseInstanceID, int studentID)
        {
            return Ok(new { Messgae = MessageFunctions.GetNotImplementedString() });
        }
    }
}
