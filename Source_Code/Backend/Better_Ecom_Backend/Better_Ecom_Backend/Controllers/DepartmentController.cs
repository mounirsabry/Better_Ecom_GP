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
    public class DepartmentController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IDataAccess _data;

        public DepartmentController(IConfiguration config, IDataAccess data)
        {
            _config = config;
            _data = data;
        }

        /// <summary>
        /// Gets the available departments.
        /// </summary>
        /// <returns>Ok result containing the departments if successful BadRequest otherwise.</returns>
        [Authorize]
        [HttpGet("GetDepartments")]
        public IActionResult GetDepartments()
        {
            //ALL USERS FUNCTION.
            string sql = "SELECT * FROM department;";

            List<Department> departments = _data.LoadData<Department, dynamic>(sql, new { }, _config.GetConnectionString("Default"));

            if (departments == null)
            {
                return BadRequest(new { Message = "operation failed." });
            }
            else
            {
                return Ok(departments);
            }
        }

        /// <summary>
        /// Student inputs the department priorities.
        /// </summary>
        /// <param name="inputData">json object containing student id and 5 priorities.</param>
        /// <returns>Ok action result if successful BadRequest otherwise.</returns>
        [Authorize(Roles = "student")]
        [HttpPost("ChooseDepartments")]
        public IActionResult ChooseDepartments([FromBody] JsonElement jsonData)
        {
            //STUDENT ONLY FUNCTION.

            if (!SetPriorityListRequiredDataValid(jsonData))
            {
                return BadRequest(new { Message = "required data missing or invalid." });
            }

            int studentID = jsonData.GetProperty("StudentID").GetInt32();
            if (HelperFunctions.GetUserTypeFromID(studentID) != "student")
            {
                return BadRequest(new { Message = "invalid student id." });
            }

            List<string> sqlList = new();
            List<dynamic> parameterList = new();

            string sql = "SELECT student_id FROM student WHERE student_id = @ID;";

            List<int> students = _data.LoadData<int, dynamic>(sql, new { ID = studentID }, _config.GetConnectionString("Default"));
            if (students is null)
            {
                return BadRequest(new { Message = "operation failed." });
            }
            if (students.Count == 0)
            {
                return BadRequest(new { Message = "id does not exist or not a student." });
            }

            if (CheckUserHasPriorities(studentID))
            {
                for (int i = 1; i <= 5; i++)
                {
                    sqlList.Add($"UPDATE student_department_priority SET priority = @priority WHERE student_id = @studentID AND department_code = @department_code");
                    parameterList.Add(new { studentID, department_code = jsonData.GetProperty($"DepartmentCode{i}").GetString(), priority = i });
                }
            }
            else
            {
                for (int i = 1; i <= 5; i++)
                {
                    sqlList.Add($"INSERT INTO student_department_priority VALUES(@studentID, @department_code, @priority)");
                    parameterList.Add(new { studentID, department_code = jsonData.GetProperty($"DepartmentCode{i}").GetString(), priority = i });
                }
            }

            List<int> states = _data.SaveDataTransaction(sqlList, parameterList, _config.GetConnectionString("Default"));
            if (states.Contains(-1))
            {
                return BadRequest(new { Message = "operation failed." });
            }
            else
            {
                return Ok();
            }
        }

        /// <summary>
        /// Get the department priority list of a student.
        /// admin can get any student.
        /// student get their prioirity list only.
        /// </summary>
        /// <param name="studentID">student id.</param>
        /// <returns>Ok result with the priorities BadRequest otherwise.</returns>
        [Authorize(Roles = "admin, student")]
        [HttpGet("GetStudentDepartmentsPriorityList/{StudentID:int}")]
        public IActionResult GetStudentDepartmentsPriorityList(int studentID)
        {
            //ADMIN, STUDENT FUNCTION.
            //Admin enters any student ID, get the priority list.
            //Student enters his ID, get the priority list that he entered.
            //Get the list from the database and return it.
            if (HelperFunctions.GetUserTypeFromID(studentID) != "student")
            {
                return BadRequest("invalid student id.");
            }

            string sql = "SELECT department_code, priority FROM student_department_priority WHERE student_id = @id;";
            dynamic rows = _data.LoadData<dynamic, dynamic>(sql, new { id = studentID }, _config.GetConnectionString("Default"));
            if (rows == null)
            {
                return BadRequest(new { Message = "unknown error, maybe database server is down." });
            }
            else
            {
                return Ok(rows);
            }
        }

        [Authorize(Roles = "admin")]
        [HttpPatch("SetDepartmentForStudent")]
        public IActionResult SetDepartmentForStudent([FromBody] dynamic inputData)
        {
            //ADMIN ONLY FUNCTION.
            JsonElement jsonData = (JsonElement)inputData;

            if (!SetDepartmentForStudentDataExist(jsonData))
            {
                return BadRequest(new { Message = "some data is missing." });
            }

            int studentID = jsonData.GetProperty("StudentID").GetInt32();
            string departmentCode = jsonData.GetProperty("DepartmentCode").GetString();

            if (HelperFunctions.GetUserTypeFromID(studentID) != "student")
            {
                return BadRequest(new { Message = "invalid student id." });
            }
            else if (!HelperFunctions.IsDepartmentCodeValid(_config, _data, departmentCode))
            {
                return BadRequest(new { Message = "invalid department code." });
            }

            string sql = "SELECT * FROM student INNER JOIN system_user" + "\n"
                    + "WHERE student.student_id = system_user.system_user_id" + "\n"
                    + "AND system_user.system_user_id = @ID;";

            Student student = _data.LoadData<Student, dynamic>(sql, new { ID = studentID }, _config.GetConnectionString("Default")).FirstOrDefault();
            student.Department_code = departmentCode;

            if (student != null)
            {
                string studentUpdateSql = "UPDATE student SET department_code = @Department_code WHERE student_id = @Student_id;";
                int state = _data.SaveData<Student>(studentUpdateSql, student, _config.GetConnectionString("Default"));

                if (state > 0)
                {
                    return Ok(student);
                }
                else
                {
                    return BadRequest(new { Message = "operation failed." });
                }
            }
            else
            {
                return BadRequest(new { Message = "id does not exist." });
            }
        }

        [Authorize]
        [HttpGet("GetDepartmentCourses/{DepartmentCode}")]
        public IActionResult GetDepartmentCourses(string departmentCode)
        {
            //Availabe for all users.
            if (!HelperFunctions.IsDepartmentCodeValid(_config, _data, departmentCode))
            {
                return BadRequest(new { Message = "invalid department code." });
            }

            string getDepartmentCoursesSql = "SELECT * FROM course WHERE department_code = @departmentCode;";
            List<Course> courses = _data.LoadData<Course, dynamic>(getDepartmentCoursesSql, new { departmentCode }, _config.GetConnectionString("Default"));

            if (courses is not null)
            {
                return Ok(courses);
            }
            else
            {
                return BadRequest(new { Message = "unknown error, maybe database server is down." });
            }
        }

        /// <summary>
        /// Gets course info by course code.
        /// </summary>
        /// <param name="courseCode">The primary key of the course</param>
        /// <returns>List containing the course as first element.</returns>
        [Authorize]
        [HttpGet("GetCourseInfoByCode/{CourseCode}")]
        public IActionResult GetCourseInfoByCode(string courseCode)
        {
            //ALL USERS FUNCTION.
            string getCourseSQL = "SELECT * FROM course WHERE course_code = @courseCode;";
            string getCoursePrerequisitesSQL = "SELECT prerequisite_course_code from course_prerequisite WHERE course_code = @courseCode;";
            string getCourseDepartmentApplicabiliteSQL = "SELECT department_code FROM course_department_applicability WHERE course_code = @courseCode;";
            List<Course> courses = _data.LoadData<Course, dynamic>(getCourseSQL, new { courseCode }, _config.GetConnectionString("Default"));
            List<string> prerequisites = _data.LoadData<string, dynamic>(getCoursePrerequisitesSQL, new { courseCode }, _config.GetConnectionString("Default"));
            List<string> departmentApplicabilities = _data.LoadData<string, dynamic>(getCourseDepartmentApplicabiliteSQL, new { courseCode }, _config.GetConnectionString("Default"));

            if (courses is null || prerequisites is null || departmentApplicabilities is null)
            {
                return BadRequest(new { Message = "unknown error, maybe database server is down." });
            }
            else
            {
                Course course = courses[0];
                CourseInfo courseInfo = new(course, prerequisites, departmentApplicabilities);
                return Ok(courseInfo);
            }
        }

        /// <summary>
        /// Gets All courses with a given name.
        /// </summary>
        /// <param name="courseName">The name of the courses to be searched</param>
        /// <returns>Ok response with list of found courses with the given name or BadRequest.</returns>
        [Authorize]
        [HttpGet("GetCourseInfoByName/{CourseName}")]
        public IActionResult GetCourseInfoByName(string courseName)
        {
            //ALL USERS FUNCTION.
            string getCourseSQL = "SELECT * FROM course WHERE course_name = @courseName";
            string getCoursePrerequisitesSQL = "SELECT prerequisite_course_code from course_prerequisite WHERE course_code = @courseCode;";
            string getCourseDepartmentApplicabiliteSQL = "SELECT department_code FROM course_department_applicability WHERE course_code = @courseCode;";
            List<Course> courses = _data.LoadData<Course, dynamic>(getCourseSQL, new { courseName }, _config.GetConnectionString("Default"));

            if (courses is null)
            {
                return BadRequest(new { Message = "unknown error, maybe database server is down." });
            }

            List<CourseInfo> coursesInfo = new();
            foreach (Course course in courses)
            {
                List<string> prerequisites = _data.LoadData<string, dynamic>(getCoursePrerequisitesSQL, new { courseCode = course.Course_code }, _config.GetConnectionString("Default"));
                List<string> departmentApplicabilities = _data.LoadData<string, dynamic>(getCourseDepartmentApplicabiliteSQL, new { courseCode = course.Course_code }, _config.GetConnectionString("Default"));

                if (prerequisites is null || departmentApplicabilities is null)
                {
                    return BadRequest(new { Message = "unknown error, maybe database server is down." });
                }
                CourseInfo courseInfo = new(course, prerequisites, departmentApplicabilities);
                coursesInfo.Add(courseInfo);
            }

            return Ok(coursesInfo);
            //May return zero course, one course, or more than one course.
        }

        /// <summary>
        /// This function takes Course object data and stores it in database.
        /// </summary>
        /// <param name="jsonData">a json object contains course data.</param>
        /// <returns>Created and course object if course is created BadRequest if failed</returns>
        [Authorize(Roles = "admin")]
        [HttpPost("AddCourseToDepartment")]
        public IActionResult AddCourseToDepartment([FromBody] JsonElement jsonData)
        {
            //ADMIN ONLY FUNCTION.

            if (!AddCourseToDepartmentRequiredDataValid(jsonData))
                return BadRequest(new { Message = "course data is not valid or not complete." });

            Course newCourse = new(jsonData);

            if (CheckCourseExist(newCourse.Course_code))
                return BadRequest(new { Message = "course already exist." });

            //Insert course.
            string insertCourseSql = "INSERT INTO course(department_code, course_code, course_name, academic_year, course_description) " +
                "VALUES( @department_code, @course_code, @course_name, @academic_year, @course_description)";

            List<string> prerequisites = new();
            List<string> departmentApplicability = new();


            if (jsonData.TryGetProperty("Prerequisites", out JsonElement temp))
            {
                if (temp.ValueKind != JsonValueKind.Array)
                {
                    return BadRequest(new { Message = "prerequisites has to be array." });
                }

                foreach (JsonElement element in temp.EnumerateArray())
                {
                    prerequisites.Add(element.GetString());
                }
            }
            if (jsonData.TryGetProperty("DepartmentApplicability", out temp))
            {
                if (temp.ValueKind != JsonValueKind.Array)
                {
                    return BadRequest(new { Message = "department applicability has to be list." });
                }
                foreach (JsonElement element in temp.EnumerateArray())
                {
                    departmentApplicability.Add(element.GetString());
                }
            }
            else
            {
                return BadRequest(new { Message = "department applicability list can not be empty." });
            }

            List<string> sqlList = new();
            List<dynamic> parameterList = new();
            sqlList.Add(insertCourseSql);
            parameterList.Add(newCourse);

            for (int i = 0; i < prerequisites.Count; i++)
            {
                sqlList.Add($"INSERT INTO course_prerequisite VALUES(@course_code, @prerequisite_course);");
                parameterList.Add(new { newCourse.Course_code, prerequisite_course = prerequisites[i] });
            }

            for (int i = 0; i < departmentApplicability.Count; i++)
            {
                sqlList.Add($"INSERT INTO course_department_applicability VALUES(@course_code, @department_code);");
                parameterList.Add(new { newCourse.Course_code, department_code = departmentApplicability[i] });
            }

            List<int> status = _data.SaveDataTransaction<dynamic>(sqlList, parameterList, _config.GetConnectionString("Default"));

            if (!status.Contains(-1))
            {
                return Created("/Department/AddCourse", newCourse);
            }
            else
            {
                return BadRequest("unknown error, maybe database server is down.");
            }
        }

        /// <summary>
        /// Updates the course info.
        /// </summary>
        /// <param name="jsonData">json object containing: admin id, course code, course name, academic year, department code, course description.</param>
        /// <returns>if successful: Ok with Course object with updated info. BadRequest otherwise.</returns>
        [Authorize(Roles = "admin")]
        [HttpPatch("UpdateCourseInfo")]
        public IActionResult UpdateCourseInfo([FromBody] JsonElement jsonData)
        {
            //ADMIN ONLY.
            if (!jsonData.TryGetProperty("UserID", out JsonElement temp) || !CheckAdminExists(temp.GetInt32()))
            {
                return BadRequest(new { Message = "user id was not provided or is invalid." });
            }

            if (!CheckUpdateCourseInfoExist(jsonData))
            {
                return BadRequest(new { Message = "you have not sent all data." });
            }

            string courseCode = jsonData.GetProperty("Course_code").GetString();

            string getCourseSql = "SELECT * FROM course WHERE course_code = @courseCode;";
            List<Course> courses = _data.LoadData<Course, dynamic>(getCourseSql, new { courseCode }, _config.GetConnectionString("Default"));

            if (courses is null)
            {
                return BadRequest(new { Message = "unknown error, maybe database server is down." });
            }

            Course course = courses.FirstOrDefault();

            if (course is null)
            {
                return BadRequest(new { Message = "course does not exist." });
            }

            course.Department_code = jsonData.GetProperty("Department_code").GetString();
            course.Course_name = jsonData.GetProperty("Course_name").GetString();
            course.Academic_year = jsonData.GetProperty("Academic_year").GetInt32();
            course.Course_description = jsonData.GetProperty("Course_description").GetString();

            string saveCourseSql = "UPDATE course SET course_name = @Course_name, department_code = @Department_code, academic_year = @Academic_year, course_description = @Course_description" + "\n"
                + "WHERE course_code = @Course_code;";

            int status = _data.SaveData(saveCourseSql, course, _config.GetConnectionString("Default"));

            if (status >= 0)
            {
                return Ok(course);
            }
            else
            {
                return BadRequest(new { Message = "unknown error, maybe database server is down." });
            }
        }


        /// <summary>
        /// Updates the prerequisites of a course.
        /// </summary>
        /// <param name="jsonData">json object containing: prerequisites list, course code, admin id.</param>
        /// <returns>if successful: Ok with course code and new prerequisite lise. BadRequest Otherwise.</returns>
        [Authorize(Roles = "admin")]
        [HttpPatch("UpdateCoursePrerequisites")]
        public IActionResult UpdateCoursePrerequisities([FromBody] JsonElement jsonData)
        {
            //ADMIN ONLY.

            if (!jsonData.TryGetProperty("UserID", out JsonElement temp) || !CheckAdminExists(temp.GetInt32()))
            {
                return BadRequest(new { Message = "user id was not provided or is invalid." });
            }

            if (!CheckUpdateCoursePrerequisitiesRequiredDataValid(jsonData))
            {

                return BadRequest(new { Message = "you have not sent all required data." });
            }

            List<string> sqlList = new();
            List<dynamic> parameterList = new();

            string courseCode = jsonData.GetProperty("Course_code").GetString();

            List<string> prerequisites = new();
            foreach (JsonElement element in jsonData.GetProperty("Prerequisites").EnumerateArray())
            {
                prerequisites.Add(element.GetString());
            }

            string deletePrerequisiteSql = "DELETE FROM course_prerequisite WHERE course_code = @courseCode;";
            string insertPrerequisiteSql = "INSERT INTO course_prerequisite VALUES(@courseCode, @Prerequisite_course_code);";

            sqlList.Add(deletePrerequisiteSql);
            parameterList.Add(new { courseCode });

            foreach (string prerequisite_course_code in prerequisites)
            {
                sqlList.Add(insertPrerequisiteSql);
                parameterList.Add(new { courseCode, prerequisite_course_code });
            }

            List<int> status = _data.SaveDataTransaction(sqlList, parameterList, _config.GetConnectionString("Default"));

            if (!status.Contains(-1))
            {

                return Ok(new { courseCode, prerequisites });
            }
            else
            {
                return BadRequest(new { Message = "unknown error, maybe database server is down." });
            }
        }

        /// <summary>
        /// Updates course applicable departments.
        /// </summary>
        /// <param name="jsonData">json object containing course containing: course code, applicability list.</param>
        /// <returns>if successful: Ok with course code and new applicability list. BadRequest otherwise.</returns>
        [Authorize(Roles = "admin")]
        [HttpPatch("UpdateCourseDepartmentApplicability")]
        public IActionResult UpdateCourseDepartmentApplicability([FromBody] JsonElement jsonData)
        {
            //ADMIN ONLY.
            if (!jsonData.TryGetProperty("UserID", out JsonElement temp) || !CheckAdminExists(temp.GetInt32()))
            {
                return BadRequest(new { Message = "user id was not provided or is invalid." });
            }
            if (!CheckUpdateDepartmentApplicabilityDataExist(jsonData))
            {
                return BadRequest(new { Message = "you have not sent all required data." });
            }

            List<string> sqlList = new();
            List<dynamic> parameterList = new();

            string courseCode = jsonData.GetProperty("Course_code").GetString();

            List<string> departmentApplicability = new();

            List<string> availableDepartments = GetDepartmentsCodes();
            foreach (JsonElement element in jsonData.GetProperty("DepartmentApplicability").EnumerateArray())
            {
                string departmentCode = element.GetString();
                if (!availableDepartments.Contains(departmentCode))
                {
                    return BadRequest(new { Message = "a provided department is not valid." });
                }

                departmentApplicability.Add(departmentCode);
            }

            string deletePrerequisiteSql = "DELETE FROM course_department_applicability WHERE course_code = @courseCode;";
            string insertPrerequisiteSql = "INSERT INTO course_department_applicability VALUES(@courseCode, @departmentCode);";

            sqlList.Add(deletePrerequisiteSql);
            parameterList.Add(new { courseCode });

            foreach (string departmentCode in departmentApplicability)
            {
                sqlList.Add(insertPrerequisiteSql);
                parameterList.Add(new { courseCode, departmentCode });
            }

            List<int> status = _data.SaveDataTransaction(sqlList, parameterList, _config.GetConnectionString("Default"));

            if (!status.Contains(-1))
            {

                return Ok(new { courseCode, departmentApplicability });
            }
            else
            {
                return BadRequest(new { Message = "unknown error, maybe database server is down." });
            }
        }

        /// <summary>
        /// Archives a course.
        /// </summary>
        /// <param name="jsonData">json object containing the course id admin with to archive.</param>
        /// <returns></returns>
        [Authorize(Roles = "admin")]
        [HttpDelete("ArchiveCourse/{CourseCode}")]
        public IActionResult ArchiveCourse(string courseCode)
        {
            //ADMIN ONLY FUNCTION.



            List<Course> course = CheckCourseArchiveStatus(courseCode);

            if (course is not null && course.Count > 0 && !course.First().Is_archived)
            {
                string archiveCourseSql = "UPDATE course SET is_archived = TRUE WHERE course_code = @courseCode;";
                int status = _data.SaveData(archiveCourseSql, new { courseCode }, _config.GetConnectionString("Default"));
                if (status >= 0)
                {
                    return Ok(new { Message = "course archived." });
                }
                else
                {
                    return BadRequest(new { Message = "unknown error, maybe database server is down." });
                }
            }
            else
            {
                if (course is null)
                {
                    return BadRequest(new { Message = "unknown error, maybe database server is down." });
                }
                else if (course.Count == 0)
                {
                    return BadRequest(new { Message = "course does not exist." });
                }
                else if (course.First().Is_archived)
                {
                    return BadRequest(new { Message = "course already archived." });
                }
            }

            return BadRequest(new { Message = "unknown error." });
        }

        /// <summary>
        /// Gets available terms.
        /// </summary>
        /// <returns>List of available terms.</returns>
        [Authorize]
        [HttpGet("GetAvailableTerms")]
        public IActionResult GetAvailableTerms()
        {
            //ALL USERS FUNCTION.
            List<Term> availableTerms = TimeUtilities.GetAvailableTerms();
            return Ok(availableTerms);
        }

        /// <summary>
        /// Gets all instances of a given course.
        /// </summary>
        /// <param name="courseCode">course code.</param>
        /// <returns>if successful:Ok with a list containing all instances of given course. BadRequest otherwise.</returns>
        [Authorize]
        [HttpGet("GetCourseInstancesFromCourse/{CourseCode}")]
        public IActionResult GetCourseInstancesFromCourse(string courseCode)
        {
            //ALL USERS FUNCTION.
            if (string.IsNullOrEmpty(courseCode))
            {
                return BadRequest(new { Message = "course code can not be empty or null." });
            }

            string getCourseInstancesByCodeSql = "SELECT * FROM course_instance WHERE course_code = @courseCode;";
            List<Course_instance> courseInstances = _data.LoadData<Course_instance, dynamic>(getCourseInstancesByCodeSql, new { courseCode }, _config.GetConnectionString("Default"));
            if (courseInstances is null)
            {

                return BadRequest(new { Message = "unknown error, maybe database server is down." });
            }
            else
            {
                return Ok(courseInstances);
            }
        }

        /// <summary>
        /// Gets course instance by it's id.
        /// </summary>
        /// <param name="instanceID">id of the course instance.</param>
        /// <returns>if successful:Ok with course instance. BadRequest Otherwise.</returns>
        [Authorize]
        [HttpGet("GetCourseInstanceByID/{InstanceID:int}")]
        public IActionResult GetCourseInstanceByID(int instanceID)
        {
            //ALL USERS FUNCTION.
            if (instanceID <= 0)
            {
                return BadRequest(new { Message = "invalid instance id." });
            }

            string getCourseInstanceByIdSql = "SELECT * FROM course_instance WHERE instance_id = @instanceID;";
            List<Course_instance> courseInstance = _data.LoadData<Course_instance, dynamic>(getCourseInstanceByIdSql, new { instanceID }, _config.GetConnectionString("Default"));
            if (courseInstance is null)
            {
                return BadRequest(new { Message = "unknown error, maybe database server is down." });
            }
            else
            {
                return Ok(courseInstance);
            }
        }

        /// <summary>
        /// Adds course instance.
        /// </summary>
        /// <param name="jsonData">json object containing course instance.</param>
        /// <returns>if successful: Created with the created course instance. BadRequest Otherwise.</returns>
        [Authorize(Roles = "admin")]
        [HttpPost("AddCourseInstance")]
        public IActionResult AddCourseInstance([FromBody] JsonElement jsonData)
        {
            //ADMIN ONLY FUNCTION.
            if (!jsonData.TryGetProperty("UserID", out JsonElement temp) || !CheckAdminExists(temp.GetInt32()))
            {
                return BadRequest(new { Message = "user id is invalid." });
            }

            if (!CheckAddCourseInstanceDataExist(jsonData))
            {
                return BadRequest(new { Message = "you have not sent all required data." });
            }

            Course_instance courseInstance = new(jsonData);

            if (!CheckCourseExist(courseInstance.Course_code))
            {
                return BadRequest("course does not exist.");
            }

            if (courseInstance.Course_year == -1)
            {
                courseInstance.Course_year = TimeUtilities.GetCurrentYear();
            }


            string addCourseInstanceSql = "INSERT INTO course_instance VALUES(NULL, @Course_code, @Course_year, @Course_term, @Credit_hours, FALSE, FALSE);";
            int status = _data.SaveData(addCourseInstanceSql, courseInstance, _config.GetConnectionString("Default"));

            if (status > 0)
            {
                return Created("Department/AddCourseInstance", courseInstance);
            }
            else
            {
                return BadRequest(new { Message = "unknown error, maybe database server is down." });
            }
        }

        [Authorize]
        [HttpGet("GetIsCourseInstanceOpenForRegistration/{CourseInstanceID:int}")]
        public IActionResult GetIsCourseInstanceOpenForRegistration(int courseInstanceID)
        {
            //STUDENT, INSTRUCTOR, ADMIN FUNCTION.

            if (ExistanceFunctions.IsCourseInstanceExists(_config, _data, courseInstanceID) == false)
            {
                return BadRequest(new { Message = MessageFunctions.GetCourseInstanceNotFoundMessage() });
            }

            List<bool> availabilities = GetCourseInstanceClosedForRegistration(courseInstanceID);

            if (availabilities is null)
            {
                return BadRequest(new { Message = MessageFunctions.GetMaybeDatabaseIsDownMessage() });
            }

            return Ok(!availabilities.First());


        }

        [Authorize(Roles = "admin")]
        [HttpPatch("MarkCourseInstanceAsClosedForRegistration")]
        public IActionResult MarkCourseInstanceAsClosedForRegistration([FromBody] JsonElement jsonInput)
        {
            //ADMIN ONLY FUNCTION.
            if (!jsonInput.TryGetProperty("CourseInstanceID", out JsonElement temp) || !temp.TryGetInt32(out int courseInstanceID))
            {
                return BadRequest(new { Message = MessageFunctions.GetRequiredDataMissingOrInvalidMessage() });
            }
            if (ExistanceFunctions.IsCourseInstanceExists(_config, _data, courseInstanceID) == false)
            {
                return BadRequest(new { Message = MessageFunctions.GetCourseInstanceNotFoundMessage() });
            }

            string markCourseInstanceClosedSql = "UPDATE course_instance SET is_closed_for_registration = TRUE WHERE instance_id = @courseInstanceID;";

            int status = _data.SaveData(markCourseInstanceClosedSql, new { courseInstanceID }, _config.GetConnectionString("Default"));

            if (status >= 0)
            {
                return Ok();
            }
            else
            {
                return BadRequest(new { Message = MessageFunctions.GetMaybeDatabaseIsDownMessage() });
            }

        }

        [Authorize(Roles = "admin")]
        [HttpPatch("RemoveClosedForRegistratiolnMarkFromCourseInstance")]
        public IActionResult RemoveClosedForRegistratiolnMarkFromCourseInstance([FromBody] JsonElement jsonInput)
        {
            //ADMIN ONLY FUNCTION.
            if (!jsonInput.TryGetProperty("CourseInstanceID", out JsonElement temp) || !temp.TryGetInt32(out int courseInstanceID))
            {
                return BadRequest(new { Message = MessageFunctions.GetRequiredDataMissingOrInvalidMessage() });
            }
            if (ExistanceFunctions.IsCourseInstanceExists(_config, _data, courseInstanceID) == false)
            {
                return BadRequest(new { Message = MessageFunctions.GetCourseInstanceNotFoundMessage() });
            }

            string markCourseInstanceClosedSql = "UPDATE course_instance SET is_closed_for_registration = FALSE WHERE instance_id = @courseInstanceID;";

            int status = _data.SaveData(markCourseInstanceClosedSql, new { courseInstanceID }, _config.GetConnectionString("Default"));

            if (status >= 0)
            {
                return Ok();
            }
            else
            {
                return BadRequest(new { Message = MessageFunctions.GetMaybeDatabaseIsDownMessage() });
            }
        }

        private static bool CheckUpdateCourseInfoExist(JsonElement jsonData)
        {
            return jsonData.TryGetProperty("Course_code", out JsonElement temp) && temp.ValueKind == JsonValueKind.String
                && jsonData.TryGetProperty("Academic_year", out temp) && temp.TryGetInt32(out _)
                && jsonData.TryGetProperty("Course_name", out temp) && temp.ValueKind == JsonValueKind.String
                && jsonData.TryGetProperty("Department_code", out temp) && temp.ValueKind == JsonValueKind.String
                && jsonData.TryGetProperty("Course_description", out temp) && temp.ValueKind == JsonValueKind.String;
        }

        private static bool CheckAddCourseInstanceDataExist(JsonElement jsonData)
        {
            return jsonData.TryGetProperty("Course_code", out JsonElement temp) && temp.ValueKind == JsonValueKind.String
                && jsonData.TryGetProperty("Credit_hours", out temp) && temp.ValueKind == JsonValueKind.Number;
        }

        private static bool CheckUpdateCoursePrerequisitiesRequiredDataValid(JsonElement jsonData)
        {
            return jsonData.TryGetProperty("Course_code", out JsonElement temp) && temp.ValueKind == JsonValueKind.String
                && jsonData.TryGetProperty("Prerequisites", out temp) && temp.ValueKind == JsonValueKind.Array;
        }

        private static bool CheckUpdateDepartmentApplicabilityDataExist(JsonElement jsonData)
        {
            return jsonData.TryGetProperty("Course_code", out JsonElement temp) && temp.ValueKind == JsonValueKind.String
                && jsonData.TryGetProperty("DepartmentApplicability", out temp) && temp.ValueKind == JsonValueKind.Array;
        }

        private bool CheckUserHasPriorities(int studentID)
        {
            string loadPrioritiesSql = "SELECT * FROM student_department_priority WHERE student_id = @studentID;";
            List<Student_department_priority> priorities = _data.LoadData<Student_department_priority, dynamic>(loadPrioritiesSql, new { studentID }, _config.GetConnectionString("Default"));

            if (priorities is null || priorities.Count == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private static bool SetDepartmentForStudentDataExist(JsonElement sentData)
        {
            return sentData.TryGetProperty("StudentID", out JsonElement temp) && temp.TryGetInt32(out _)
            && sentData.TryGetProperty("DepartmentCode", out temp) && temp.ValueKind == JsonValueKind.String;
        }

        private bool CheckCourseExist(string courseCode)
        {
            List<string> codes = _data.LoadData<string, dynamic>("SELECT course_code from course WHERE course_code = @courseCode;", new { courseCode }, _config.GetConnectionString("Default"));
            return codes != null && codes.Count > 0;
        }
        private static bool SetPriorityListRequiredDataValid(JsonElement sentData)
        {
            return sentData.TryGetProperty("StudentID", out JsonElement temp) && temp.TryGetInt32(out _)
            && sentData.TryGetProperty("DepartmentCode1", out temp) && temp.ValueKind == JsonValueKind.String
            && sentData.TryGetProperty("DepartmentCode2", out temp) && temp.ValueKind == JsonValueKind.String
            && sentData.TryGetProperty("DepartmentCode3", out temp) && temp.ValueKind == JsonValueKind.String
            && sentData.TryGetProperty("DepartmentCode4", out temp) && temp.ValueKind == JsonValueKind.String
            && sentData.TryGetProperty("DepartmentCode5", out temp) && temp.ValueKind == JsonValueKind.String;
        }

        private bool CheckAdminExists(int ID)
        {
            string checkAdminSql = "SELECT admin_user_id FROM admin_user INNER JOIN system_user on system_user_id = admin_user_id where admin_user_id = @ID";

            List<int> ids = _data.LoadData<int, dynamic>(checkAdminSql, new { ID }, _config.GetConnectionString("Default"));
            if (ids is null || ids.Count == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        private bool AddCourseToDepartmentRequiredDataValid(JsonElement sentData)
        {
            return sentData.TryGetProperty("Course_code", out JsonElement temp) && temp.ValueKind == JsonValueKind.String
                && sentData.TryGetProperty("DepartmentApplicability", out temp) && temp.ValueKind == JsonValueKind.Array
                && (sentData.TryGetProperty("Department_code", out temp) && temp.ValueKind == JsonValueKind.String && GetDepartmentsCodes().Contains(temp.GetString()))
                && sentData.TryGetProperty("Course_name", out temp) && temp.ValueKind == JsonValueKind.String;
        }

        private List<Course> CheckCourseArchiveStatus(string code)
        {
            string checkCourseSql = "SELECT course_code, is_archived FROM course  where course_code = @code";
            List<Course> courses = _data.LoadData<Course, dynamic>(checkCourseSql, new { code }, _config.GetConnectionString("Default"));

            return courses;
        }

        private List<bool> GetCourseInstanceClosedForRegistration(int courseInstanceID)
        {
            string getRegistrationAvailabilitySql = "SELECT is_closed_for_registration FROM course_instance WHERE instance_id = @courseInstanceID;";
            List<bool> availablilityList = _data.LoadData<bool, dynamic>(getRegistrationAvailabilitySql, new { courseInstanceID }, _config.GetConnectionString("Default"));
            return availablilityList;
        }

        private List<string> GetDepartmentsCodes()
        {
            string sql = "SELECT department_code FROM department;";
            List<string> departments = _data.LoadData<string, dynamic>(sql, new { }, _config.GetConnectionString("Default"));

            if (departments == null)
            {
                return new List<string>();
            }
            else
            {
                return departments;
            }
        }
    }
}
