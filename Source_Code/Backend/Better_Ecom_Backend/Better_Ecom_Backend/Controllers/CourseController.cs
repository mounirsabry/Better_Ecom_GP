using Better_Ecom_Backend.Helpers;
using Better_Ecom_Backend.Models;
using DataLibrary;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text.Json;

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

        [Authorize]
        [HttpGet("GetCourseAvailableCourseInstances/{CourseCode}")]
        public IActionResult GetCourseAvailableCourseInstances([FromHeader] string Authorization, string courseCode)
        {
            //STUDENT, INSTRUCTOR, ADMIN FUNCTION.

            return Ok(new { Message = HelperFunctions.GetNotImplementedString() });
        }

        [Authorize(Roles = "admin, student")]
        [HttpGet("GetStudentRegisteredCourses/{StudentID:int}")]
        public IActionResult GetStudentRegisteredCourses([FromHeader] string Authorization, int studentID)
        {
            TokenInfo info = HelperFunctions.GetIdAndTypeFromToken(Authorization);
            if (info.Type == "student" && info.UserID != studentID)
            {
                return Forbid("students can only get their own data.");
            }
            string getCourseCodes = "SELECT course_code FROM course_instance " + "\n" +
                "INNER JOIN student_course_instance_registration ON student_course_instance_registration.course_instance_id = course_instance.instance_id WHERE student_id = @studentID;";
            string getCourseAvailableCourseInstances = $"SELECT * FROM course WHERE course_code in ({getCourseCodes})";

            List<Course> courses = _data.LoadData<Course,dynamic>(getCourseAvailableCourseInstances, new { studentID }, _config.GetConnectionString("Default"));

            if(courses is null)
            {
                return BadRequest(new { Message = HelperFunctions.GetMaybeDatabaseIsDownMessage() });
            }

            return Ok(courses);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Authorization"></param>
        /// <param name="studentID"></param>
        /// <returns></returns>
        [Authorize(Roles = "admin, student")]
        [HttpGet("GetStudentRegisteredCourseInstances/{Student:int}")]
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Authorization"></param>
        /// <param name="studentID"></param>
        /// <param name="courseCode"></param>
        /// <returns></returns>
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

            string getStudentRegisteredInstanceIds = "SELECT course_instance_id FROM student_course_instance_registration WHERE student_id = @studentID;";
            string getCourseStudentRegisteredCourseInstancesSql = $"SELECT * FROM course_instance WHERE instance_id IN ({getStudentRegisteredInstanceIds}) AND course_code = @courseCode;";

            List<Course_instance> instances = _data.LoadData<Course_instance, dynamic>(getCourseStudentRegisteredCourseInstancesSql, new { studentID, courseCode }, _config.GetConnectionString("Default"));

            if(instances is null)
            {
                return BadRequest(new { Message = HelperFunctions.GetMaybeDatabaseIsDownMessage() });
            }

            return Ok(instances);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="Authorization"></param>
        /// <param name="jsonInput"></param>
        /// <returns></returns>
        [Authorize(Roles = "student, admin")]
        [HttpPost("RegisterToCourseInstance")]
        public IActionResult RegisterToCourseInstance([FromHeader] string Authorization, [FromBody] JsonElement jsonInput)
        {
            //STUDENT, ADMIN FUNCTION.
            TokenInfo info = HelperFunctions.GetIdAndTypeFromToken(Authorization);

            if (AppSettingsFunctions.GetIsCourseRegistrationOpen(_config) == false)
            {
                if (info.Type == "student")
                {
                    return BadRequest(new { Message = "normal course registration is unavailable, try check late course registration." });
                }
                else
                {
                    if (AppSettingsFunctions.GetIsLateCourseRegistrationOpen(_config))
                    {
                        return BadRequest(new { Message = "course registration is closed, but late registration is open, the student must register through requests." });
                    }
                }
            }

            if (!RegisterToCourseInstanceDataValid(jsonInput))
            {
                return BadRequest(new { Message = HelperFunctions.GetRequiredDataMissingOrInvalidMessage() });
            }

            int studentID = jsonInput.GetProperty("StudentID").GetInt32();
            int courseInstanceID = jsonInput.GetProperty("CourseInstanceID").GetInt32();
            if (info.Type == "student" && info.UserID != studentID)
            {
                return Forbid("students can only get their own data.");
            }

            string courseCode;
            List<string> courseCodes = GetCourseCodesListFromCourseInstanceID(courseInstanceID);
            if (courseCodes is null)
            {
                return BadRequest(new { Message = HelperFunctions.GetMaybeDatabaseIsDownMessage() });
            }
            else if (courseCodes.Count == 0)
            {
                return BadRequest(new { Message = "no course found associated with the given instance id." });
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
                registration.Registration_id = GetRegistrationId(courseInstanceID, studentID);
                return Ok(registration);
            }
            else
            {
                return BadRequest(new { Message = HelperFunctions.GetMaybeDatabaseIsDownMessage() });
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="Authorization"></param>
        /// <param name="jsonInput"></param>
        /// <returns></returns>
        [Authorize(Roles ="student, admin")]
        [HttpDelete("DropStudentFromCourseInstance")]
        public IActionResult DropStudentFromCourseInstance([FromHeader] string Authorization,[FromBody] JsonElement jsonInput)
        {
            //STUDENT, ADMIN FUNCTION.

            if(!DropStudentFromCourseInstanceDataValid(jsonInput))
            {
                return BadRequest(new { Message = HelperFunctions.GetRequiredDataMissingOrInvalidMessage() });
            }

            int studentID = jsonInput.GetProperty("StudentID").GetInt32();
            int courseInstanceID = jsonInput.GetProperty("CourseInstanceID").GetInt32();


            TokenInfo info = HelperFunctions.GetIdAndTypeFromToken(Authorization);
            if (info.Type == "student" && info.UserID != studentID)
            {
                return Forbid("students can only get their own data.");
            }

            string getTargetCourseInstanceYearAndTermSql = "SELECT course_year, course_term FROM course_instance WHERE instance_id = @courseInstanceID;";

            var term = _data.LoadData<dynamic, dynamic>(getTargetCourseInstanceYearAndTermSql, new { courseInstanceID }, _config.GetConnectionString("Default"));

            if(term is null)
            {
                return BadRequest(new { Message = HelperFunctions.GetMaybeDatabaseIsDownMessage() });
            }
            else if(term.Count == 0)
            {
                return BadRequest(new { Message = "instance doesn't exist." });
            }
            else if(term[0].course_year != TimeUtilities.GetCurrentYear() || term[0].course_term != TimeUtilities.GetCurrentTerm())
            {
                return BadRequest(new { Message = "can't drop from old course" });
            }

            string dropStudentFromCourseInstanceSql = "DELETE FROM student_course_instance_registration WHERE course_instance_id = @courseInstanceID AND student_id = @studentID;";

            int status = _data.SaveData(dropStudentFromCourseInstanceSql, new { courseInstanceID, studentID }, _config.GetConnectionString("Default"));

            if(status > 0)
            {
                return Ok();
            }
            else
            {
                return BadRequest(new { Message = HelperFunctions.GetMaybeDatabaseIsDownMessage() });
            }

        }

        [Authorize]
        [HttpGet("GetCourseInstanceRegisteredStudents/{CourseInstanceID:int}")]
        public IActionResult GetCourseInstanceRegisteredStudents(int courseInstanceID)
        {
            //ALL UESRS.

            string getCourseInstanceRegisteredStudentsIdsSql = "SELECT student_id FROM student_course_instance_registration, course_instance_late_registration_request" + "\n" +
                "WHERE course_instance_id = @courseInstanceID";

            string getCourseInstanceRegisteredStudentsSql = "SELECT * FROM student" + "\n" +
                "INNER JOIN system_user on system_user_id = student_id" + "\n" +
                $"WHERE student_id in ({getCourseInstanceRegisteredStudentsIdsSql})";

            List<Student> students = _data.LoadData<Student, dynamic>(getCourseInstanceRegisteredStudentsSql, new { courseInstanceID }, _config.GetConnectionString("Default"));

            if(students is null)
            {
                return BadRequest(new { Message = HelperFunctions.GetMaybeDatabaseIsDownMessage() });
            }
            

            return Ok(students);
        }

        [Authorize(Roles = "admin")]
        [HttpGet("GetAllLateCourseInstanceRegistrationRequests")]
        public IActionResult GetAllLateCourseInstanceRegistrationRequests()
        {
            //ADMIN ONLY FUNCTION.

            List<Course_instance_late_registration_request> lateCourseInstances = _data.LoadData<Course_instance_late_registration_request, dynamic>("SELECT * FROM course_instance_late_registration_request;",new { },_config.GetConnectionString("Default"));

            if(lateCourseInstances is null)
            {
                return BadRequest(new { Message = HelperFunctions.GetMaybeDatabaseIsDownMessage() });
            }

            return Ok(lateCourseInstances);
        }

        [Authorize(Roles ="admin")]
        [HttpGet("GetCourseLateCourseRegistrationRequests/{CourseCode}")]
        public IActionResult GetCourseLateCourseRegistrationRequests(string courseCode)
        {
            //ADMIN ONLY FUNCTION.

            string getCourseInstancesSql = "SELECT instance_id FROM course_instance WHERE course_code = @courseCode;";
            string getCourseLateRegistrationRequest = $"SELECT * FROM course_instance_late_registration_request WHERE course_instance_id in ({getCourseInstancesSql});";
            List<Course_instance_late_registration_request> lateCourseInstances = _data.LoadData<Course_instance_late_registration_request, dynamic>(getCourseLateRegistrationRequest, new { courseCode }, _config.GetConnectionString("Default"));

            if (lateCourseInstances is null)
            {
                return BadRequest(new { Message = HelperFunctions.GetMaybeDatabaseIsDownMessage() });
            }

            return Ok(lateCourseInstances);
        }

        [Authorize(Roles = "student, admin")]
        [HttpGet("GetStudentLateCourseInstanceRegistrationRequests/{StudentID:int}")]
        public IActionResult GetStudentLateCourseInstanceRegistrationRequests([FromHeader]string Authorization, int studentID)
        {
            //STUDENT, ADMIN FUNCTION.
            TokenInfo info = HelperFunctions.GetIdAndTypeFromToken(Authorization);
            if (info.Type == "student" && info.UserID != studentID)
            {
                return Forbid("students can only get their own data.");
            }

            string getStudentLateCourseInstanceRegistrationRequestsSql = "SELECT * FROM course_instance_late_registration_request WHERE student_id = @studentID;";
            List<Course_instance_late_registration_request> lateRegistrations = _data.LoadData<Course_instance_late_registration_request, dynamic>(
                getStudentLateCourseInstanceRegistrationRequestsSql, new { studentID }, _config.GetConnectionString("Default"));

            if(lateRegistrations is null)
            {
                return BadRequest(new { Message = HelperFunctions.GetMaybeDatabaseIsDownMessage() });
            }
            return Ok(lateRegistrations);
        }

        [HttpGet("GetLateCourseInstanceRegistrationRequestAvailableStauts")]
        public IActionResult GetLateCourseInstanceRegistrationRequestAvailableStauts()
        {
            //STUDENT, ADMIN FUNCTION.
            return Ok(EnumFunctions.GetLateRegistrationRequestStausList());
        }

        [HttpPost("SubmitLateCourseInstanceRegistrationRequest")]
        public IActionResult SubmitLateCourseInstanceRegistrationRequest([FromBody] JsonElement jsonInput)
        {
            //STUDENT ONLY FUNCTION.
            if(!SubmitLateCourseInstanceRegistrationRequestDataValid(jsonInput))
            {
                return BadRequest(new { Message = HelperFunctions.GetRequiredDataMissingOrInvalidMessage() });
            }

            int studentID = jsonInput.GetProperty("StudentID").GetInt32();
            int courseInstanceID = jsonInput.GetProperty("CourseInstanceID").GetInt32();
            DateTime requestDate = DateTime.Now;
            LateRegistrationRequestStatus requestStatus = LateRegistrationRequestStatus.Pending_Accept;

            string getTargetCourseInstanceYearAndTermSql = "SELECT course_year, course_term FROM course_instance WHERE instance_id = @courseInstanceID;";

            var term = _data.LoadData<dynamic, dynamic>(getTargetCourseInstanceYearAndTermSql, new { courseInstanceID }, _config.GetConnectionString("Default"));

            if (term is null)
            {
                return BadRequest(new { Message = HelperFunctions.GetMaybeDatabaseIsDownMessage() });
            }
            else if (term.Count == 0)
            {
                return BadRequest(new { Message = "instance doesn't exist." });
            }
            else if (term[0].course_year != TimeUtilities.GetCurrentYear() || term[0].course_term != TimeUtilities.GetCurrentTerm())
            {
                return BadRequest(new { Message = "can't register in old course" });
            }

            string submitLateCourseInstanceRegistrationRequest = "INSERT INTO course_instance_late_registration_request VALUES(NULL, @studentID, @courseInstanceID, @requestDate, @requestStatus);";

            dynamic parameters = new
            {
                studentID,
                courseInstanceID,
                requestDate,
                requestStatus = nameof(requestStatus)
            };

            int status = _data.SaveData(submitLateCourseInstanceRegistrationRequest, parameters, _config.GetConnectionString("Default"));

            if(status > 0)
            {
                int id = GetLateRegistrationId(courseInstanceID, studentID);

                Course_instance_late_registration_request registration = new(id, studentID, courseInstanceID, requestDate, requestStatus);

                return Ok(registration);
            }
            else
            {

                return BadRequest(new { Message = HelperFunctions.GetMaybeDatabaseIsDownMessage() });

            }

        }


        [Authorize(Roles = "student")]
        [HttpDelete("DeleteLateCourseInstanceRegistrationRequest")]
        public IActionResult DeleteLateCourseInstanceRegistrationRequest([FromBody] JsonElement jsonInput)
        {
            //STUDENT ONLY FUNCTION.
            if (!jsonInput.TryGetProperty("lateRegistrationRequestID", out JsonElement temp) || !temp.TryGetInt32(out int requestID))
            {
                return BadRequest(new { Message = HelperFunctions.GetRequiredDataMissingOrInvalidMessage() });
            }

            string deleteLateCourseInstanceRegistrationRequestSql = "DELETE FROM course_instance_late_registration_request WHERE request_id = @requestID;";

            int status = _data.SaveData(deleteLateCourseInstanceRegistrationRequestSql, new { requestID }, _config.GetConnectionString("Default"));

            if (status > 0)
            {
                return Ok();
            }
            else
            {
                return BadRequest(new { Message = HelperFunctions.GetMaybeDatabaseIsDownMessage() });
            }



        }

        [Authorize(Roles ="admin")]
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Authorization"></param>
        /// <param name="studentID"></param>
        /// <param name="courseInstanceID"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("GetStudentCourseInstanceStatus/{StudentID:int}/{CourseInstanceID:int}")]
        public IActionResult GetStudentCourseInstanceStatus([FromBody] string Authorization, int studentID, int courseInstanceID)
        {
            //ALL USERS.
            TokenInfo info = HelperFunctions.GetIdAndTypeFromToken(Authorization);
            if (info.Type == "student" && info.UserID != studentID)
            {
                return Forbid("students can only get their own data.");
            }

            string getStudentCourseInstanceStatusSql = "SELECT student_course_intance_status FROM student_course_instance_registration WHERE student_id = @studentID AND course_instance_id = @courseInstanceID;";

            List<StudentCourseInstanceRegistrationStatus> registrationStatuses = _data.LoadData<StudentCourseInstanceRegistrationStatus, dynamic>(getStudentCourseInstanceStatusSql,
                new { studentID, courseInstanceID }, _config.GetConnectionString("Default"));
            
            if(registrationStatuses is null)
            {
                return BadRequest(new { Message = HelperFunctions.GetMaybeDatabaseIsDownMessage() });
            }
            else
            {
                return Ok(registrationStatuses);
            }
        }

        [Authorize(Roles ="admin")]
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
            Console.WriteLine(workingYear);
            Console.WriteLine(sql);

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

        private int GetLateRegistrationId(int courseInstanceID, int studentID)
        {
            List<int> ids = _data.LoadData<int, dynamic>("SELECT request_id FROM course_instance_late_registration_request WHERE student_id = @studentID AND course_instance_id = @courseInstanceID;",
                new { courseInstanceID, studentID }, _config.GetConnectionString("Default"));
            if(ids is null || ids.Contains(-1))
            {
                return -1;
            }
            return ids[0];
        }

        private int GetRegistrationId(int courseInstanceID, int studentID)
        {
            List<int> ids = _data.LoadData<int, dynamic>("SELECT registration_id FROM student_course_instance_registration WHERE student_id = @studentID AND course_instance_id = @courseInstanceID;",
                new { courseInstanceID, studentID }, _config.GetConnectionString("Default"));
            if (ids is null || ids.Contains(-1))
            {
                return -1;
            }
            return ids.FirstOrDefault();
        }

        private bool RegisterToCourseInstanceDataValid(JsonElement jsonInput)
        {
            throw new NotImplementedException();
        }

        private bool DropStudentFromCourseInstanceDataValid(JsonElement jsonInput)
        {
            return jsonInput.TryGetProperty("CourseInstanceID", out JsonElement temp) && temp.TryGetInt32(out _)
                && jsonInput.TryGetProperty("StudentID", out temp) && temp.TryGetInt32(out _);
        }

        private bool SubmitLateCourseInstanceRegistrationRequestDataValid(JsonElement jsonInput)
        {
            return jsonInput.TryGetProperty("StudentID", out JsonElement temp) && temp.TryGetInt32(out _)
                && jsonInput.TryGetProperty("CourseInstanceID", out temp) && temp.TryGetInt32(out _);
        }
    }
}
