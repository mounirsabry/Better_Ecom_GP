using Better_Ecom_Backend.Entities;
using Better_Ecom_Backend.Helpers;
using Better_Ecom_Backend.Models;
using DataLibrary;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public IActionResult GetStudentAvailableCourses([FromHeader] string Authorization, int studentID)
        {
            //STUDENT, ADMIN FUNCTION.
            TokenInfo info = HelperFunctions.GetIdAndTypeFromToken(Authorization);
            if (info.Type == "student" && info.UserID != studentID)
            {
                return Forbid("students can only get their own data.");
            }

            if (ExistanceFunctions.IsDBUpAndRunning(_config, _data) == false)
            {
                return BadRequest(new { Message = MessageFunctions.GetMaybeDatabaseIsDownMessage() });
            }
            List<Course> studentAvailableCourses = GetStudentAvailableCoursesList(studentID);
            if (studentAvailableCourses is null)
            {
                return BadRequest(new { Message = "server problem or the student does not have a department." });
            }

            return Ok(studentAvailableCourses);
        }


        [Authorize]
        [HttpGet("GetCourseAvailableCourseInstances/{CourseCode}")]
        public IActionResult GetCourseAvailableCourseInstances([FromHeader] string Authorization, string courseCode)
        {
            //STUDENT, INSTRUCTOR, ADMIN FUNCTION.

            if(ExistanceFunctions.IsCourseExists(_config,_data, courseCode) == false)
            {
                return BadRequest(new { Message = MessageFunctions.GetCourseNotFoundMessage() });
            }

            List<Course_instance> instances = GetCourseAvailableCourseInstancesList(courseCode);

            if(instances is null)
            {
                return BadRequest(new { Message = MessageFunctions.GetMaybeDatabaseIsDownMessage() });
            }

            return Ok(instances);

        }

        /// <summary>
        /// Gets the courses a given student is registered in.
        /// </summary>
        /// <param name="Authorization">Authorization token</param>
        /// <param name="studentID">The id of the student.</param>
        /// <returns>List of courses that the student is registered in</returns>
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
                "INNER JOIN student_course_instance_registration ON student_course_instance_registration.course_instance_id = course_instance.instance_id WHERE student_id = @studentID";
            string getCourseAvailableCourseInstances = $"SELECT * FROM course WHERE course_code in ({getCourseCodes})";

            List<Course> courses = _data.LoadData<Course, dynamic>(getCourseAvailableCourseInstances, new { studentID }, _config.GetConnectionString("Default"));

            if (courses is null)
            {
                return BadRequest(new { Message = MessageFunctions.GetMaybeDatabaseIsDownMessage() });
            }

            return Ok(courses);
        }

        /// <summary>
        /// Gets Course Instances that the student is registered in.
        /// </summary>
        /// <param name="Authorization">Authorization token</param>
        /// <param name="studentID">The id of the student.</param>
        /// <returns>List of course instances that the student is registered in</returns>
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

        /// <summary>
        /// Gets all instances of a course that a student was registered in.
        /// </summary>
        /// <param name="Authorization">Authorization token</param>
        /// <param name="studentID">The id of the student.</param>
        /// <param name="courseCode">The code of the course.</param>
        /// <returns>List of instances.</returns>
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

            string getStudentRegisteredInstanceIds = "SELECT course_instance_id FROM student_course_instance_registration WHERE student_id = @studentID";
            string getCourseStudentRegisteredCourseInstancesSql = $"SELECT * FROM course_instance WHERE instance_id IN ({getStudentRegisteredInstanceIds}) AND course_code = @courseCode;";

            List<Course_instance> instances = _data.LoadData<Course_instance, dynamic>(getCourseStudentRegisteredCourseInstancesSql, new { studentID, courseCode }, _config.GetConnectionString("Default"));

            if (instances is null)
            {
                return BadRequest(new { Message = MessageFunctions.GetMaybeDatabaseIsDownMessage() });
            }

            return Ok(instances);
        }

        [Authorize(Roles = "admin, student")]
        [HttpGet("GetIsNormalCourseRegistrationOpen")]
        public IActionResult GetIsNormalCourseRegistrationOpen()
        {
            return Ok(AppSettingsFunctions.GetIsNormalCourseRegistrationOpen(_config));
        }

        [Authorize(Roles = "admin, student")]
        [HttpGet("GetIsLateCourseRegistrationOpen")]
        public IActionResult GetIsLateCourseRegistrationOpen()
        {
            return Ok(AppSettingsFunctions.GetIsLateCourseRegistrationOpen(_config));
        }

        [Authorize(Roles = "admin, student")]
        [HttpGet("GetIsDropCourseRegistrationOpen")]
        public IActionResult GetIsDropCourseRegistrationOpen()
        {
            return Ok(AppSettingsFunctions.GetIsDropCourseRegistrationOpen(_config));
        }

        /// <summary>
        /// Registers Student to CourseInstance.
        /// </summary>
        /// <param name="Authorization">Authorization token.</param>
        /// <param name="jsonInput">json object contains(Student id , instance id)</param>
        /// <returns>The registration object.</returns>
        [Authorize(Roles = "student, admin")]
        [HttpPost("RegisterToCourseInstance")]
        public IActionResult RegisterToCourseInstance([FromHeader] string Authorization, [FromBody] JsonElement jsonInput)
        {
            //STUDENT, ADMIN FUNCTION.
            TokenInfo info = HelperFunctions.GetIdAndTypeFromToken(Authorization);

            if (AppSettingsFunctions.GetIsNormalCourseRegistrationOpen(_config) == false)
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
                return BadRequest(new { Message = MessageFunctions.GetRequiredDataMissingOrInvalidMessage() });
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
                return BadRequest(new { Message = MessageFunctions.GetMaybeDatabaseIsDownMessage() });
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
            var parameters = new
            {
                studentID ,
                courseInstanceID ,
                registrationDate ,
                studentCourseInstanceStatus = nameof(StudentCourseInstanceRegistrationStatus.Undertaking),
                grade = Enum.GetName(StudentCourseInstanceGrade.Not_Specified)
            };
            string insertCourseRegistrationSql = "INSERT INTO student_course_instance_registration VALUES(NULL, @studentID, @courseInstanceID, @registrationDate, @studentCourseInstanceStatus,@grade);";

            int status = _data.SaveData(insertCourseRegistrationSql, parameters, _config.GetConnectionString("Default"));

            if (status > 0)
            {
                registration.Registration_id = GetRegistrationId(courseInstanceID, studentID);
                return Ok(registration);
            }
            else
            {
                return BadRequest(new { Message = MessageFunctions.GetMaybeDatabaseIsDownMessage() });
            }
        }


        /// <summary>
        /// Delete Student Registration to course instance.
        /// </summary>
        /// <param name="Authorization">Authorization token.</param>
        /// <param name="jsonInput">json object containing(student id, intance id).</param>
        /// <returns>Ok</returns>
        [Authorize(Roles = "student, admin")]
        [HttpDelete("DropStudentFromCourseInstance/{CourseInstanceID:int}/{StudentID:int}")]
        public IActionResult DropStudentFromCourseInstance([FromHeader] string Authorization, int courseInstanceID, int studentID)
        {
            //STUDENT, ADMIN FUNCTION.




            TokenInfo info = HelperFunctions.GetIdAndTypeFromToken(Authorization);
            if (info.Type == "student" && info.UserID != studentID)
            {
                return Forbid("students can only get their own data.");
            }



            var courseInstanceClosedForRegistration = GetCourseInstanceClosedForRegistration(courseInstanceID);

            if (courseInstanceClosedForRegistration is null)
            {
                return BadRequest(new { Message = MessageFunctions.GetMaybeDatabaseIsDownMessage() });
            }
            else if (courseInstanceClosedForRegistration.Count == 0)
            {
                return BadRequest(new { Message = "instance doesn't exist." });
            }
            
            else if (GetCourseInstanceClosedForRegistration(courseInstanceID)[0])
            {
                return BadRequest(new { Message = "can't drop from old course" });
            }

            string dropStudentFromCourseInstanceSql = "DELETE FROM student_course_instance_registration WHERE course_instance_id = @courseInstanceID AND student_id = @studentID;";

            int status = _data.SaveData(dropStudentFromCourseInstanceSql, new { courseInstanceID, studentID }, _config.GetConnectionString("Default"));

            if (status >= 0)
            {
                return Ok();
            }
            else
            {
                return BadRequest(new { Message = MessageFunctions.GetMaybeDatabaseIsDownMessage() });
            }

        }

        /// <summary>
        /// Gets students registered in course instance.
        /// </summary>
        /// <param name="courseInstanceID">The id of the course instance.</param>
        /// <returns>Ok response with List of students</returns>
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

            if (students is null)
            {
                return BadRequest(new { Message = MessageFunctions.GetMaybeDatabaseIsDownMessage() });
            }


            return Ok(students);
        }


        /// <summary>
        /// Get all late course instance registration requests.
        /// </summary>
        /// <returns>List with Late course instance registration requests.</returns>
        [Authorize(Roles = "admin")]
        [HttpGet("GetAllLateCourseInstanceRegistrationRequests")]
        public IActionResult GetAllLateCourseInstanceRegistrationRequests()
        {
            //ADMIN ONLY FUNCTION.

            List<Course_instance_late_registration_request> lateCourseInstancesRegistrationRequests = _data.LoadData<Course_instance_late_registration_request, dynamic>("SELECT * FROM course_instance_late_registration_request;", new { }, _config.GetConnectionString("Default"));

            if (lateCourseInstancesRegistrationRequests is null)
            {
                return BadRequest(new { Message = MessageFunctions.GetMaybeDatabaseIsDownMessage() });
            }

            return Ok(lateCourseInstancesRegistrationRequests);
        }

        /// <summary>
        /// Gets Late registration request of all instances of a given course.
        /// </summary>
        /// <param name="courseCode">code of the course</param>
        /// <returns>List with registration requests.</returns>
        [Authorize(Roles = "admin")]
        [HttpGet("GetCourseLateCourseRegistrationRequests/{CourseCode}")]
        public IActionResult GetCourseLateCourseRegistrationRequests(string courseCode)
        {
            //ADMIN ONLY FUNCTION.

            string getCourseInstancesSql = "SELECT instance_id FROM course_instance WHERE course_code = @courseCode";
            string getCourseLateRegistrationRequest = $"SELECT * FROM course_instance_late_registration_request WHERE course_instance_id in ({getCourseInstancesSql});";
            List<Course_instance_late_registration_request> lateCourseInstancesRegistrationRequests = _data.LoadData<Course_instance_late_registration_request, dynamic>(getCourseLateRegistrationRequest, new { courseCode }, _config.GetConnectionString("Default"));

            if (lateCourseInstancesRegistrationRequests is null)
            {
                return BadRequest(new { Message = MessageFunctions.GetMaybeDatabaseIsDownMessage() });
            }

            return Ok(lateCourseInstancesRegistrationRequests);
        }

        /// <summary>
        /// Gets Late request of a given student to a course instance.
        /// </summary>
        /// <param name="Authorization">Authorization token.</param>
        /// <param name="studentID">Id</param>
        /// <returns>Ok with late request.</returns>
        [Authorize(Roles = "student, admin")]
        [HttpGet("GetStudentLateCourseInstanceRegistrationRequests/{StudentID:int}")]
        public IActionResult GetStudentLateCourseInstanceRegistrationRequests([FromHeader] string Authorization, int studentID)
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

            if (lateRegistrations is null)
            {
                return BadRequest(new { Message = MessageFunctions.GetMaybeDatabaseIsDownMessage() });
            }
            return Ok(lateRegistrations);
        }

        [HttpGet("GetLateCourseInstanceRegistrationRequestAvailableStauts")]
        public IActionResult GetLateCourseInstanceRegistrationRequestAvailableStauts()
        {
            //STUDENT, ADMIN FUNCTION.
            return Ok(EnumFunctions.GetLateRegistrationRequestStausList());
        }

        /// <summary>
        /// Submits late registration request to a course instance.
        /// </summary>
        /// <param name="jsonInput"></param>
        /// <returns>The registration request.</returns>
        [Authorize(Roles ="student")]
        [HttpPost("SubmitLateCourseInstanceRegistrationRequest")]
        public IActionResult SubmitLateCourseInstanceRegistrationRequest([FromBody] JsonElement jsonInput)
        {
            //STUDENT ONLY FUNCTION.
            if (!SubmitLateCourseInstanceRegistrationRequestDataValid(jsonInput))
            {
                return BadRequest(new { Message = MessageFunctions.GetRequiredDataMissingOrInvalidMessage() });
            }

            int studentID = jsonInput.GetProperty("StudentID").GetInt32();
            int courseInstanceID = jsonInput.GetProperty("CourseInstanceID").GetInt32();
            DateTime requestDate = DateTime.Now;
            LateRegistrationRequestStatus requestStatus = LateRegistrationRequestStatus.Pending_Accept;

            if (ExistanceFunctions.IsStudentExists(_config, _data, studentID) == false)
            {
                return BadRequest(new { Message = MessageFunctions.GetStudentNotFoundMessage() });
            }
            else if (ExistanceFunctions.IsCourseInstanceExists(_config, _data, courseInstanceID) == false)
            {
                return BadRequest(new { Message = MessageFunctions.GetCourseNotFoundMessage() });
            }

            List<bool> availableList = GetCourseInstanceClosedForRegistration(courseInstanceID);

            if (availableList is null)
            {
                return BadRequest();
            }

            if (availableList.First())
            {
                return BadRequest(new { Message = MessageFunctions.GetCourseInstanceIsClosedMessage() });
            }

            string submitLateCourseInstanceRegistrationRequest = "INSERT INTO course_instance_late_registration_request VALUES(NULL, @studentID, @courseInstanceID, @requestDate, @requestStatus);";

            dynamic parameters = new
            {
                studentID,
                courseInstanceID,
                requestDate,
                requestStatus = Enum.GetName<LateRegistrationRequestStatus>(requestStatus)
            };

            int status = _data.SaveData(submitLateCourseInstanceRegistrationRequest, parameters, _config.GetConnectionString("Default"));

            if (status > 0)
            {
                int id = GetLateRegistrationId(courseInstanceID, studentID);

                Course_instance_late_registration_request registration = new(id, studentID, courseInstanceID, requestDate, requestStatus);

                return Ok(registration);
            }
            else
            {

                return BadRequest(new { Message = MessageFunctions.GetMaybeDatabaseIsDownMessage() });

            }

        }

        /// <summary>
        /// Deletes Late course registration request to a give course instance by a given student.
        /// </summary>
        /// <param name="jsonInput">json data containing request id.</param>
        /// <returns>Ok</returns>
        [Authorize(Roles = "student")]
        [HttpDelete("DeleteLateCourseInstanceRegistrationRequest/{RequestID:int}")]
        public IActionResult DeleteLateCourseInstanceRegistrationRequest(int requestID)
        {
            //STUDENT ONLY FUNCTION.




            string deleteLateCourseInstanceRegistrationRequestSql = "DELETE FROM course_instance_late_registration_request WHERE request_id = @requestID;";

            int status = _data.SaveData(deleteLateCourseInstanceRegistrationRequestSql, new { requestID }, _config.GetConnectionString("Default"));

            if (status >= 0)
            {
                return Ok();
            }
            else
            {
                return BadRequest(new { Message = MessageFunctions.GetMaybeDatabaseIsDownMessage() });
            }



        }

        /// <summary>
        /// Changes Late Course Registration request.
        /// If accepted inserts new course registration instance with student data.
        /// </summary>
        /// <param name="jsonInput">json object containing RequestID: id of registration request, and RequestStatus: new request Status.</param>
        /// <returns></returns>
        [Authorize(Roles = "admin")]
        [HttpPatch("SetLateCourseInstanceRegistrationRequest")]
        public IActionResult SetLateCourseInstanceRegistrationRequest([FromBody] JsonElement jsonInput)
        {
            //ADMIN ONLY FUNCTION.

            if (!SetLateCourseInstanceRegistrationRequestDataValid(jsonInput))
            {
                return BadRequest(new { Message = MessageFunctions.GetRequiredDataMissingOrInvalidMessage() });
            }

            int requestID = jsonInput.GetProperty("RequestID").GetInt32();
            LateRegistrationRequestStatus requestStatus = (LateRegistrationRequestStatus)jsonInput.GetProperty("RequestStatus").GetInt32();

            List<string> sqlList = new();
            List<dynamic> parametersList = new();

            string getCourseLateRegistrationRequestSql = "SELECT * FROM course_instance_late_registration_request WHERE request_id = @requestID;";
            List<Course_instance_late_registration_request> registrations = _data.LoadData<Course_instance_late_registration_request, dynamic>(getCourseLateRegistrationRequestSql,
                new { requestID }, _config.GetConnectionString("Default"));

            if (registrations is null)
            {
                return BadRequest(new { Message = MessageFunctions.GetMaybeDatabaseIsDownMessage() });
            }

            Course_instance_late_registration_request registration = registrations.FirstOrDefault();
            if (registration is null)
            {
                return BadRequest(new { Message = "registration does not exist." });
            }

            string setLateCourseInstanceRegistrationRequestStatusSql = "UPDATE course_instance_late_registration_request SET request_status = @requestStatus WHERE request_id = @requestID;";

            sqlList.Add(setLateCourseInstanceRegistrationRequestStatusSql);
            parametersList.Add(new { requestID, requestStatus = Enum.GetName<LateRegistrationRequestStatus>(requestStatus) });



            if (requestStatus == LateRegistrationRequestStatus.Accepted && registration.Request_status != LateRegistrationRequestStatus.Accepted)
            {
                string insertCourseRegistration = "INSERT INTO student_course_instance_registration VALUES(NULL, @studentID, @courseInstanceID, @registrationDate, @studentCourseInstanceStatus,@grade);";
                var parameters = new
                {
                    studentID = registration.Student_id,
                    courseInstanceID = registration.Course_instance_id,
                    registrationDate = DateTime.Now,
                    studentCourseInstanceStatus = nameof(StudentCourseInstanceRegistrationStatus.Undertaking),
                    grade = Enum.GetName(StudentCourseInstanceGrade.Not_Specified) 
                };
                sqlList.Add(insertCourseRegistration);
                parametersList.Add(parameters);
            }
            else if (registration.Request_status == LateRegistrationRequestStatus.Accepted)
            {
                return BadRequest(new { Message = "can not change accepted student." });
            }

            List<int> status = _data.SaveDataTransaction(sqlList, parametersList, _config.GetConnectionString("Default"));

            if (status.Contains(-1))
            {
                return BadRequest(new { Message = MessageFunctions.GetMaybeDatabaseIsDownMessage() });
            }

            registration.Request_status = requestStatus;

            return Ok(registration);
        }



        [Authorize]
        [HttpGet("GetStudentCourseInstanceRegistrationAvailableStatus")]
        public IActionResult GetStudentCourseInstanceRegistrationAvailableStatus()
        {
            //ALL USERS.
            return Ok(EnumFunctions.GetStudentCourseInstanceRegistrationStatusList());
        }

        /// <summary>
        /// Gets Student status regarding a given course instance.
        /// </summary>
        /// <param name="Authorization">Authorization token.</param>
        /// <param name="studentID">id of the student.</param>
        /// <param name="courseInstanceID">id of course instance.</param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("GetStudentCourseInstanceStatus/{StudentID:int}/{CourseInstanceID:int}")]
        public IActionResult GetStudentCourseInstanceStatus([FromHeader] string Authorization, int studentID, int courseInstanceID)
        {
            //ALL USERS.
            TokenInfo info = HelperFunctions.GetIdAndTypeFromToken(Authorization);
            if (info.Type == "student" && info.UserID != studentID)
            {
                return Forbid("students can only get their own data.");
            }

            string getStudentCourseInstanceStatusSql = "SELECT student_course_instance_status FROM student_course_instance_registration WHERE student_id = @studentID AND course_instance_id = @courseInstanceID;";

            List<string> registrationStatuses = _data.LoadData<string, dynamic>(getStudentCourseInstanceStatusSql,
                new { studentID, courseInstanceID }, _config.GetConnectionString("Default"));

            if (registrationStatuses is null)
            {
                return BadRequest(new { Message = MessageFunctions.GetMaybeDatabaseIsDownMessage() });
            }
            else if (registrationStatuses.Count == 0)
            {
                return BadRequest(new { Message = "student has no status for given course instance." });
            }
            else
            {
                List<StudentCourseInstanceRegistrationStatus> status = new() { Enum.Parse<StudentCourseInstanceRegistrationStatus>(registrationStatuses.First()) };
                return Ok(status);
            }
        }

        /// <summary>
        /// Set status for student status regarding a course instance.
        /// </summary>
        /// <param name="jsonInput">json object containing student registration id and status.</param>
        /// <returns>json object containing student registration id and new status.</returns>
        [Authorize(Roles = "admin")]
        [HttpPatch("SetStudentCourseInstanceStatus")]
        public IActionResult SetStudentCourseInstanceStatus([FromBody] JsonElement jsonInput)
        {
            //ADMIN ONLY FUNCTION.
            if (!SetStudentCourseInstanceStatusDataValid(jsonInput))
            {
                return BadRequest(new { Message = MessageFunctions.GetRequiredDataMissingOrInvalidMessage() });
            }

            int courseInstanceRegistrationID = jsonInput.GetProperty("CourseInstanceRegistrationID").GetInt32();
            StudentCourseInstanceRegistrationStatus courseStatus = (StudentCourseInstanceRegistrationStatus)jsonInput.GetProperty("CourseStatus").GetInt32();

            string getCourseInstanceRegistrationSql = "SELECT * FROM student_course_instance_registration WHERE registration_id = @courseInstanceRegistrationID;";
            List<Student_course_instance_registration> registrations = _data.LoadData<Student_course_instance_registration, dynamic>(getCourseInstanceRegistrationSql, new { courseInstanceRegistrationID },
                _config.GetConnectionString("Default"));

            if (registrations is null)
            {
                return BadRequest(new { Message = MessageFunctions.GetMaybeDatabaseIsDownMessage() });
            }

            Student_course_instance_registration registration = registrations.FirstOrDefault();

            if (registration is null)
            {
                return BadRequest(new { Message = "registration does not exist." });
            }

            string setCourseStatusSql = "UPDATE student_course_instance_registration SET student_course_instance_status = @courseStatus WHERE registration_id = @courseInstanceRegistrationID;";

            int status = _data.SaveData(setCourseStatusSql, new { courseStatus = Enum.GetName<StudentCourseInstanceRegistrationStatus>(courseStatus), courseInstanceRegistrationID }, _config.GetConnectionString("Default"));

            if (status >= 0)
            {
                registration.Student_course_instance_status = courseStatus;
                return Ok(registration);
            }
            else
            {

                return BadRequest(new { Message = MessageFunctions.GetMaybeDatabaseIsDownMessage() });
            }

        }

        /// <summary>
        /// Get courses that a given instructor is registered in
        /// </summary>
        /// <param name="Authorization">Authorization token.</param>
        /// <param name="instructorID">id of instructor.</param>
        /// <returns></returns>
        [Authorize(Roles = "admin, instructor")]
        [HttpGet("GetInstructorRegisteredCourses/{InstructorID:int}")]
        public IActionResult GetInstructorRegisteredCourses([FromHeader] String Authorization, int instructorID)
        {
            //Return the course codes list.
            TokenInfo tokenInfo = HelperFunctions.GetIdAndTypeFromToken(Authorization);
            string userType = tokenInfo.Type;
            int tokenInstructorID = tokenInfo.UserID;
            if (userType == "instructor")
            {
                if (tokenInstructorID != instructorID)
                {
                    return Forbid("instructors can only get their data.");
                }
            }
            if (ExistanceFunctions.IsInstructorExists(_config, _data, instructorID) == false)
            {
                return BadRequest(new { Message = MessageFunctions.GetInstructorNotFoundMessage() });
            }

            string getRegisteredCourseInstanceIdsSql = "SELECT course_instance_id FROM instructor_course_instance_registration WHERE instructor_id = @instructorID ";
            string getCourseCodesSql = $"SELECT DISTINCT course_code FROM course_instance WHERE instance_id IN ({getRegisteredCourseInstanceIdsSql})";
            string getCoursesSql = $"SELECT * FROM course WHERE course_code in ({getCourseCodesSql});";

            List<Course> courseCodes = _data.LoadData<Course, dynamic>(getCoursesSql, new { instructorID }, _config.GetConnectionString("Default"));

            if (courseCodes is null)
            {
                return BadRequest(new { Message = MessageFunctions.GetMaybeDatabaseIsDownMessage() });
            }
            else
            {
                return Ok(courseCodes);
            }

        }

        /// <summary>
        /// Gets Course Intstances That a given instructor is registered in.
        /// </summary>
        /// <param name="Authorization">Authorization token.</param>
        /// <param name="instructorID">Id of instructor.</param>
        /// <returns>List with course instance ids.</returns>
        [Authorize(Roles = "admin, instructor")]
        [HttpGet("GetInstructorRegisteredCourseInstances/{InstructorID:int}")]
        public IActionResult GetInstructorRegisteredCourseInstances([FromHeader] String Authorization, int instructorID)
        {
            //Return the course instances IDs list.
            TokenInfo tokenInfo = HelperFunctions.GetIdAndTypeFromToken(Authorization);
            string userType = tokenInfo.Type;
            int tokenInstructorID = tokenInfo.UserID;
            if (userType == "instructor")
            {
                if (tokenInstructorID != instructorID)
                {
                    return Forbid("instructors can only get their data.");
                }
            }
            if (ExistanceFunctions.IsInstructorExists(_config, _data, instructorID) == false)
            {
                return BadRequest(new { Message = MessageFunctions.GetInstructorNotFoundMessage() });
            }

            string getRegisteredCourseInstanceIdsSql = "SELECT course_instance_id FROM instructor_course_instance_registration WHERE instructor_id = @instructorID ";
            string getCourseInstancesFromIds = $"Select * FROM course_instance WHERE instance_id IN ({getRegisteredCourseInstanceIdsSql});";
            List<Course_instance> courseInstancesIds = _data.LoadData<Course_instance, dynamic>(getCourseInstancesFromIds, new { instructorID }, _config.GetConnectionString("Default"));

            if (courseInstancesIds is null)
            {
                return BadRequest(new { Message = MessageFunctions.GetMaybeDatabaseIsDownMessage() });
            }
            else
            {
                return Ok(courseInstancesIds);
            }
        }
        /// <summary>
        /// Gets the instances of a given course that a given instructor is registered in.
        /// </summary>
        /// <param name="Authorization">Authorization token.</param>
        /// <param name="instructorID">Id of instructor.</param>
        /// <param name="courseCode">Course code.</param>
        /// <returns></returns>
        [Authorize(Roles = "admin, instructor")]
        [HttpGet("GetCourseInstructorRegisteredCourseInstances/{InstructorID:int}/{CourseCode}")]
        public IActionResult GetCourseInstructorRegisteredCourseInstances([FromHeader] string Authorization, int instructorID, string courseCode)
        {
            TokenInfo tokenInfo = HelperFunctions.GetIdAndTypeFromToken(Authorization);
            string userType = tokenInfo.Type;
            int tokenInstructorID = tokenInfo.UserID;
            if (userType == "instructor")
            {
                if (tokenInstructorID != instructorID)
                {
                    return Forbid("instructors can only get their data.");
                }
            }

            if (ExistanceFunctions.IsCourseExists(_config, _data, courseCode) == false)
            {
                return BadRequest(new { Message = MessageFunctions.GetCourseNotFoundMessage() });
            }

            string getInstructorRegisteredCourseInstanceIdsSql = "SELECT instance_id FROM instructor_course_instance_registration WHERE intructor_id = @instructorID";

            string getInstructorRegisteredCourseCodesSql = $"SELECT * FROM course_instance WHERE instance_id IN ({getInstructorRegisteredCourseInstanceIdsSql}) AND course_code = @courseCode;";

            List<Course_instance> instanceIds = _data.LoadData<Course_instance, dynamic>(getInstructorRegisteredCourseCodesSql, new { instructorID, courseCode }, _config.GetConnectionString("Default"));

            if (instanceIds is null)
            {
                return BadRequest(new { Message = MessageFunctions.GetMaybeDatabaseIsDownMessage() });
            }

            //Return all the course instances that the instructor is to in the specified course.
            return Ok(instanceIds);
        }

        /// <summary>
        /// Register instructor to given course.
        /// </summary>
        /// <param name="jsonInput">json object containing instructor id and course instance id.</param>
        /// <returns>Ok</returns>
        [Authorize(Roles = "admin")]
        [HttpPost("RegisterInstructorToCourseInstance")]
        public IActionResult RegisterInstructorToCourseInstance([FromBody] JsonElement jsonInput)
        {
            //The admin registers the instructor to the spcified course instance.
            if (RegisterInstructorToCourseInstanceDataValid(jsonInput) == false)
            {
                return BadRequest(new { Message = MessageFunctions.GetRequiredDataMissingOrInvalidMessage() });
            }

            int instructorID = jsonInput.GetProperty("InstructorID").GetInt32();
            int courseInstanceID = jsonInput.GetProperty("CourseInstanceID").GetInt32();
            DateTime registrationDate = DateTime.Now;

            if (ExistanceFunctions.IsInstructorExists(_config, _data, instructorID) == false)
            {
                return BadRequest(new { Message = MessageFunctions.GetInstructorNotFoundMessage() });
            }
            if (ExistanceFunctions.IsCourseInstanceExists(_config, _data, courseInstanceID) == false)
            {
                return BadRequest(new { Message = MessageFunctions.GetCourseInstanceNotFoundMessage() });
            }

            string insertInstructorRegistrationSql = "INSERT INTO instructor_course_instance_registration VALUES(NULL, @instructorID, @courseInstanceID, @registrationDate);";
            int status = _data.SaveData(insertInstructorRegistrationSql, new { instructorID, courseInstanceID, registrationDate }, _config.GetConnectionString("Default"));

            if (status > 0)
            {
                return Ok();
            }
            else
            {
                return BadRequest(new { Message = MessageFunctions.GetMaybeDatabaseIsDownMessage() });
            }
        }

        /// <summary>
        /// Drops a registered instructor from a given course.
        /// </summary>
        /// <param name="jsonInput">json object containing(intructor is , course instance id).</param>
        /// <returns>Ok</returns>
        [Authorize(Roles = "admin")]
        [HttpDelete("DropInstructorFromCourseInstance/{CourseInstanceID:int}/{InstructorID:int}")]
        public IActionResult DropInstructorFromCourseInstance(int courseInstanceID, int instructorID)
        {
            //The admin drops the instructor for the specified course instance.


            if (ExistanceFunctions.IsCourseInstanceExists(_config, _data, courseInstanceID) == false)
            {
                return BadRequest(new { Message = MessageFunctions.GetCourseInstanceNotFoundMessage() });
            }

            List<bool> availablilities = GetCourseInstanceClosedForRegistration(courseInstanceID);
            if (availablilities is null)
            {
                return BadRequest(new { Message = MessageFunctions.GetMaybeDatabaseIsDownMessage() });
            }

            if (availablilities.First())
            {
                return BadRequest(new { Message = MessageFunctions.GetCourseInstanceIsClosedMessage() });
            }

            string deleteIntructorRegistrationSql = "DELETE FROM instructor_course_instance_registration WHERE course_instance_id = @courseInstanceID AND instructor_id = @instructorID;";

            int status = _data.SaveData(deleteIntructorRegistrationSql, new { instructorID, courseInstanceID }, _config.GetConnectionString("Default"));

            if (status >= 0)
            {
                return Ok();
            }
            else
            {
                return BadRequest(new { Message = MessageFunctions.GetMaybeDatabaseIsDownMessage() });
            }

        }

        /// <summary>
        /// Gets the registered instructors in a given course instance.
        /// </summary>
        /// <param name="courseInstanceID">course instance id</param>
        /// <returns>List of instructors ids and names.</returns>
        [Authorize]
        [HttpGet("GetCourseInstanceRegisteredInstructors/{CourseInstanceID:int}")]
        public IActionResult GetCourseInstanceRegisteredInstructors(int courseInstanceID)
        {
            //Return all the instructors (ids, names) registered to a specific course instance.

            if (ExistanceFunctions.IsCourseInstanceExists(_config, _data, courseInstanceID) == false)
            {
                return BadRequest(new { Message = MessageFunctions.GetCourseInstanceNotFoundMessage() });
            }
            string getCourseInstanceIntructorIDsSql = "SELECT instructor_id FROM instructor_course_instance_registration WHERE course_instance_id = @courseInstanceID ";
            string getInstuctorsIdsAndNamesSql = $"SELECT system_user_id, full_name WHERE system_user_id IN ({getCourseInstanceIntructorIDsSql});";

            var instructorsIdsAndNames = _data.LoadData<dynamic, dynamic>(getInstuctorsIdsAndNamesSql, new { courseInstanceID }, _config.GetConnectionString("Default"));

            if (instructorsIdsAndNames is null)
            {
                return BadRequest(new { Message = MessageFunctions.GetMaybeDatabaseIsDownMessage() });
            }

            return Ok(instructorsIdsAndNames);
        }

        [Authorize]
        [HttpGet("GetCourseInstanceReadOnlyStatus/{CourseInstanceID:int}")]
        public IActionResult GetCourseInstanceReadOnlyStatus(int courseInstanceID)
        {
            if (ExistanceFunctions.IsDBUpAndRunning(_config, _data) == false)
            {
                return BadRequest(new { Message = MessageFunctions.GetMaybeDatabaseIsDownMessage() });
            }

            if (ExistanceFunctions.IsCourseInstanceExists(_config, _data, courseInstanceID) == false)
            {
                return BadRequest(new { Message = MessageFunctions.GetCourseInstanceNotFoundMessage() });
            }
            return Ok(HelperFunctions.GetCourseInstanceReadOnlyStatus(_config, _data, courseInstanceID));
        }

        [Authorize(Roles = "admin, instructor")]
        [HttpPatch("SetCourseInstanceReadOnlyStatus")]
        public IActionResult SetCourseInstanceReadOnlyStatus([FromHeader]string Authorization,[FromBody] JsonElement jsonInput)
        {
            if (ExistanceFunctions.IsDBUpAndRunning(_config, _data) == false)
            {
                return BadRequest(new { Message = MessageFunctions.GetMaybeDatabaseIsDownMessage() });
            }
            if(SetCourseInstanceReadOnlyStatusDataValid(jsonInput) == false)
            {
                return BadRequest(new { Message = MessageFunctions.GetRequiredDataMissingOrInvalidMessage() });
            }

            int courseInstanceID = jsonInput.GetProperty("CourseInstanceID").GetInt32();
            bool readOnlyStatus = jsonInput.GetProperty("ReadOnlyStatus").GetBoolean();

            if (ExistanceFunctions.IsCourseInstanceExists(_config, _data, courseInstanceID) == false)
            {
                return BadRequest(new { Message = MessageFunctions.GetCourseInstanceNotFoundMessage() });
            }


            TokenInfo tokenInfo = HelperFunctions.GetIdAndTypeFromToken(Authorization);
            if(tokenInfo.Type == "instructor")
            {
                if (RegistrationFunctions.IsInstructorRegisteredToCourseInstance(_config, _data, tokenInfo.UserID, courseInstanceID) == false)
                {
                    return BadRequest(new { Message = "instructor must be registered in course instance." });
                }
            }

            string setCourseInstanceReadOnlySql = "UPDATE course_instance SET is_read_only = @readOnlyStatus WHERE instance_id = @courseInstanceID; ";

            int status = _data.SaveData(setCourseInstanceReadOnlySql, new { readOnlyStatus, courseInstanceID }, _config.GetConnectionString("Default"));

            if(status >= 0)
            {
                return Ok();
            }
            else
            {
                return BadRequest(new { Message = MessageFunctions.GetMaybeDatabaseIsDownMessage() });
            }

            //Extract the id from the token, if the user is an instructor, then he must be registered to the course instance.
            //Check the status provided.
            //update the row with the new status.
            //return an error or return course instance status updated successfully.
        }



        private List<Course> GetStudentAvailableCoursesList(int studentID)
        {
            string getStudentDepartmentSQL = "SELECT department_code FROM student WHERE student_id = @StudentID;";
            List<string> departmentCodes = _data.LoadData<string, dynamic>(getStudentDepartmentSQL, new { StudentID = studentID }, _config.GetConnectionString("Default"));
            if (departmentCodes is null || departmentCodes.Count == 0)
            {
                return null;
            }
            string studentDepartmentCode = departmentCodes[0];

            string getStudentDepartmentAvailableCourseCodesSQL;
            dynamic getStudentDepartmentAvailableCourseCodesParameters;
            if (studentDepartmentCode == "GE")
            {
                getStudentDepartmentAvailableCourseCodesSQL = "SELECT course_code FROM course_department_applicability WHERE department_code = 'GE';";
                getStudentDepartmentAvailableCourseCodesParameters = new { };
            }
            else
            {
                getStudentDepartmentAvailableCourseCodesSQL = "SELECT DISTINCT(course_code) FROM course_department_applicability WHERE department_code = 'GE' OR department_code = @DepartmentCode;";
                getStudentDepartmentAvailableCourseCodesParameters = new { DepartmentCode = studentDepartmentCode };
            }

            List<string> studentDepartmentAvailableCourseCodes;
            studentDepartmentAvailableCourseCodes = _data.LoadData<string, dynamic>(getStudentDepartmentAvailableCourseCodesSQL,
                getStudentDepartmentAvailableCourseCodesParameters, _config.GetConnectionString("Default"));
            if (studentDepartmentAvailableCourseCodes is null)
            {
                return null;
            }

            List<Course> studentAvailableCourses = new();
            foreach (string courseCode in studentDepartmentAvailableCourseCodes)
            {
                //Checks if the user has previously passed the course or he is currently undertaking the course.
                string getAllCourseCourseInstancesSQL = "SELECT instance_id FROM course_instance WHERE course_code = @CourseCode";
                string getPreviousStudentAttempts = "SELECT * FROM student_course_instance_registration WHERE student_id = @StudentID" + "\n"
                                            + $"AND course_instance_id IN ({ getAllCourseCourseInstancesSQL });";

                List<Student_course_instance_registration> studentCourseAttempts;
                studentCourseAttempts = _data.LoadData<Student_course_instance_registration, dynamic>(getPreviousStudentAttempts,
                    new { CourseCode = courseCode, StudentID = studentID }, _config.GetConnectionString("Default"));
                if (studentCourseAttempts is null)
                {
                    return null;
                }

                bool IsStudentPassedOrHaveActiveAttempt = false;
                foreach (var attempt in studentCourseAttempts)
                {
                    if (attempt.Student_course_instance_status == StudentCourseInstanceRegistrationStatus.Passed
                    || attempt.Student_course_instance_status == StudentCourseInstanceRegistrationStatus.Undertaking)
                    {
                        IsStudentPassedOrHaveActiveAttempt = true;
                        break;
                    }
                }
                if (IsStudentPassedOrHaveActiveAttempt)
                {
                    continue;
                }

                string getCoursePrerequisitesSQL = "SELECT prerequisite_course_code FROM course_prerequisite WHERE course_code = @CourseCode;";

                List<string> coursePrerequisites;
                coursePrerequisites = _data.LoadData<string, dynamic>(getCoursePrerequisitesSQL, new { CourseCode = courseCode }, _config.GetConnectionString("Default"));

                if (coursePrerequisites is null)
                {
                    return null;
                }

                bool isPassedFound = true;
                foreach (string prerequisiteCourseCode in coursePrerequisites)
                {
                    string getStudentPrerequisiteCourseStatus = "SELECT student_course_instance_status FROM course" + "\n"
                                                        + "INNER JOIN course_instance" + "\n"
                                                        + "INNER JOIN student_course_instance_registration" + "\n"
                                                        + "WHERE course.course_code = course_instance.course_code" + "\n"
                                                        + "AND course_instance.instance_id = student_course_instance_registration.course_instance_id" + "\n"
                                                        + "AND course.course_code = @CourseCode;";

                    List<string> prerequisiteCourseStatusList;
                    prerequisiteCourseStatusList = _data.LoadData<string, dynamic>(getStudentPrerequisiteCourseStatus, new { CourseCode = prerequisiteCourseCode },
                        _config.GetConnectionString("Default"));

                    if (prerequisiteCourseStatusList is null)
                    {
                        return null;
                    }
                    else if (prerequisiteCourseStatusList.Count == 0)
                    {
                        isPassedFound = false;
                        break;
                    }

                    foreach (string status in prerequisiteCourseStatusList)
                    {
                        isPassedFound = false;
                        if (status == Enum.GetName(StudentCourseInstanceRegistrationStatus.Passed))
                        {
                            isPassedFound = true;
                            break;
                        }
                    }

                    if (!isPassedFound)
                    {
                        break;
                    }
                }

                if (isPassedFound)
                {
                    string GetCourseByCourseCodeSQL = "SELECT * FROM course WHERE course_code = @CourseCode;";
                    List<Course> courseList;
                    courseList = _data.LoadData<Course, dynamic>(GetCourseByCourseCodeSQL, new { CourseCode = courseCode }, _config.GetConnectionString("Default"));
                    
                    if (courseList is null || courseList.Count == 0)
                    {
                        return null;
                    }
                    Course availableCourse = courseList[0];

                    studentAvailableCourses.Add(availableCourse);
                }
            }
            return studentAvailableCourses;
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
                    + "AND course_year = @workingYear" + "\n"
                    + "AND course_term = @currentTerm" + "\n"
                    + "OR course_term = 'Other'" + "\n"
                    + "AND is_closed_for_registration = FALSE;";

            List<Course_instance> instances = _data.LoadData<Course_instance, dynamic>(sql, new { courseCode, workingYear, currentTerm }, _config.GetConnectionString("Default"));



            Console.WriteLine(workingYear);
            Console.WriteLine(sql);

            return instances;
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
            string sql = "SELECT course.course_code FROM course" + "\n"
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
            if (ids is null || ids.Contains(-1))
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

        private List<bool> GetCourseInstanceClosedForRegistration(int courseInstanceID)
        {
            string getRegistrationAvailabilitySql = "SELECT is_closed_for_registration FROM course_instance WHERE instance_id = @courseInstanceID;";
            List<bool> availablilityList = _data.LoadData<bool, dynamic>(getRegistrationAvailabilitySql, new { courseInstanceID }, _config.GetConnectionString("Default"));
            return availablilityList;
        }

        private static bool RegisterToCourseInstanceDataValid(JsonElement jsonInput)
        {
            return jsonInput.TryGetProperty("StudentID", out JsonElement temp) && temp.TryGetInt32(out _)
                && jsonInput.TryGetProperty("CourseInstanceID", out temp) && temp.TryGetInt32(out _);
        }

        private static bool DropStudentFromCourseInstanceDataValid(JsonElement jsonInput)
        {
            return jsonInput.TryGetProperty("CourseInstanceID", out JsonElement temp) && temp.TryGetInt32(out _)
                && jsonInput.TryGetProperty("StudentID", out temp) && temp.TryGetInt32(out _);
        }

        private static bool SubmitLateCourseInstanceRegistrationRequestDataValid(JsonElement jsonInput)
        {
            return jsonInput.TryGetProperty("StudentID", out JsonElement temp) && temp.TryGetInt32(out _)
                && jsonInput.TryGetProperty("CourseInstanceID", out temp) && temp.TryGetInt32(out _);
        }

        private static bool SetStudentCourseInstanceStatusDataValid(JsonElement jsonInput)
        {
            return jsonInput.TryGetProperty("StudentID", out JsonElement temp) && temp.TryGetInt32(out _)
                && jsonInput.TryGetProperty("CourseInstanceID", out temp) && temp.TryGetInt32(out _)
                && jsonInput.TryGetProperty("CourseStatus", out temp) && temp.TryGetInt32(out _);
        }

        private static bool SetLateCourseInstanceRegistrationRequestDataValid(JsonElement jsonInput)
        {
            return jsonInput.TryGetProperty("RequestID", out JsonElement temp) && temp.TryGetInt32(out _)
                && jsonInput.TryGetProperty("RequestStatus", out temp) && temp.TryGetInt32(out _);
        }

        private static bool RegisterInstructorToCourseInstanceDataValid(JsonElement jsonInput)
        {
            return jsonInput.TryGetProperty("InstructorID", out JsonElement temp) && temp.TryGetInt32(out _)
                && jsonInput.TryGetProperty("CourseInstanceID", out temp) && temp.TryGetInt32(out _);
        }

        private static bool DropInstructorFromCourseInstanceDataValid(JsonElement jsonInput)
        {
            return jsonInput.TryGetProperty("InstructorID", out JsonElement temp) && temp.TryGetInt32(out _)
                && jsonInput.TryGetProperty("CourseInstanceID", out temp) && temp.TryGetInt32(out _);
        }

        private bool SetCourseInstanceReadOnlyStatusDataValid(JsonElement jsonInput)
        {
            return jsonInput.TryGetProperty("CourseInstanceID", out JsonElement temp) && temp.TryGetInt32(out _)
                && jsonInput.TryGetProperty("ReadOnlyStatus", out temp) && (temp.ValueKind == JsonValueKind.False || temp.ValueKind == JsonValueKind.True);
        }
    }
}
