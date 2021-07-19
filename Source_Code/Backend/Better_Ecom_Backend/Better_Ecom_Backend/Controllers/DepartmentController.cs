﻿using Better_Ecom_Backend.Helpers;
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
        [HttpGet("GetDepartments")]
        [Authorize]
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
        public IActionResult ChooseDepartments([FromBody] dynamic inputData)
        {
            //STUDENT ONLY FUNCTION.
            JsonElement jsonData = (JsonElement)inputData;

            if (!SetPriorityListRequiredDataExist(jsonData))
            {
                return BadRequest(new { Message = "you have not sent all required data." });
            }

            int studentID = jsonData.GetProperty("StudentID").GetInt32();
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
                    sqlList.Add($"UPDATE student_department_priority_list SET priority = @priority WHERE student_id = @studentID AND department_code = @department_code");
                    parameterList.Add(new { studentID, department_code = jsonData.GetProperty($"DepartmentCode{i}").GetString(), priority = i });
                }
            }
            else
            {
                for (int i = 1; i <= 5; i++)
                {
                    sqlList.Add($"INSERT INTO student_department_priority_list VALUES(@studentID, @department_code, @priority)");
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
        /// <param name="id">student id.</param>
        /// <returns>Ok result with the priorities BadRequest otherwise.</returns>
        [Authorize(Roles = "admin, student")]
        [HttpGet("GetStudentPriorityList/{ID:int}")]
        public IActionResult GetStudentPriorityList(int id)
        {
            //ADMIN, STUDENT FUNCTION.
            //Admin enters any student ID, get the priority list.
            //Student enters his ID, get the priority list that he entered.
            //Get the list from the database and return it.

            string sql = "SELECT department_code, priority FROM student_department_priority_list WHERE student_id = @id;";

            dynamic rows = _data.LoadData<dynamic, dynamic>(sql, new { id }, _config.GetConnectionString("Default"));
            if (rows == null)
            {
                return BadRequest(new { Message = "unknown error, maybe database server is down." });
            }
            else if (rows.Count == 0)
            {
                return Ok(new { Message = "student did not sumbit any priority list." });
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
        [HttpGet("GetCourseInfoByCode/{CourseCode}")]
        [Authorize]
        public IActionResult GetCourseInfoByCode(string courseCode)
        {
            //ALL USERS FUNCTION.
            string getCourseSql = "SELECT * FROM course WHERE course_code = @courseCode";
            List<Course> course = _data.LoadData<Course, dynamic>(getCourseSql, new { courseCode }, _config.GetConnectionString("Default"));

            if (course is null)
                return BadRequest(new { Message = "unknown error, maybe database server is down." });
            else
                return Ok(course);

            //Returns one course, if the course code was not found, return an error.
        }

        /// <summary>
        /// Gets All courses with a given name.
        /// </summary>
        /// <param name="courseName">The name of the courses to be searched</param>
        /// <returns>Ok response with list of found courses with the given name or BadRequest.</returns>
        [HttpGet("GetCourseInfoByName/{CourseName}")]
        public IActionResult GetCourseInfoByName(string courseName)
        {
            //ALL USERS FUNCTION.
            string getCourseSql = "SELECT * FROM course WHERE course_name = @courseName";
            List<Course> course = _data.LoadData<Course, dynamic>(getCourseSql, new { courseName }, _config.GetConnectionString("Default"));

            if (course is null)
                return BadRequest(new { Message = "unknown error, maybe database server is down." });
            else
                return Ok(course);

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
            if (!jsonData.TryGetProperty("UserID", out JsonElement temp) || !CheckAdminExists(temp.GetInt32()))
                return BadRequest(new { Message = "user id was not provided or is invalid." });

            if (!CheckCourseData(jsonData))
                return BadRequest(new { Message = "course data is not valid or not complete." });

            Course newCourse = new(jsonData);


            if (CheckCourseExist(newCourse.Course_code))
                return BadRequest(new { Message = "course already exist." });



            //Insert course.
            string insertCourseSql = "INSERT INTO course(department_code, course_code, course_name, academic_year, course_description) " +
                "VALUES( @department_code, @course_code, @course_name, @academic_year, @course_description)";

            List<string> prerequisites = new();
            List<string> departmentApplicability = new();
       
            if (jsonData.TryGetProperty("prerequisites", out temp))
            {
                foreach (JsonElement element in temp.EnumerateArray())
                {
                    prerequisites.Add(element.GetString());
                }
            }
            if (jsonData.TryGetProperty("departmentApplicability", out temp))
            {

                foreach (JsonElement element in temp.EnumerateArray())
                {
                    departmentApplicability.Add(element.GetString());
                }
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
                return BadRequest("unknown error, maybe database server is down.");

        }


        /// <summary>
        /// Updates the course info.
        /// </summary>
        /// <param name="jsonData">json object containing: admin id, course code, course name, academic year, department code, course description.</param>
        /// <returns>if successful: Ok with Course object with updated info. BadRequest otherwise.</returns>
        [Authorize(Roles ="admin")]
        [HttpPatch("UpdateCourseInfo")]
        public IActionResult UpdateCourseInfo([FromBody] JsonElement jsonData)
        {
            //ADMIN ONLY.
            if (!jsonData.TryGetProperty("UserID", out JsonElement temp) || !CheckAdminExists(temp.GetInt32()))
                return BadRequest(new { Message = "user id was not provided or is invalid." });

            if (!CheckUpdateCourseInfoExist(jsonData))
                return BadRequest(new { Message = "you have not sent all data." });

            string courseCode = jsonData.GetProperty("courseCode").GetString();

            string getCourseSql = "SELECT * FROM course WHERE course_code = @courseCode;";

            List<Course> courses = _data.LoadData<Course, dynamic>(getCourseSql, new { courseCode }, _config.GetConnectionString("Default"));

            if (courses is null)
                return BadRequest(new { Message = "unknown error, maybe database server is down." });

            Course course = courses.FirstOrDefault();

            if (course is null)
                return BadRequest(new { Message = "course does not exist." });

            course.Department_code = jsonData.GetProperty("departmentCode").GetString();
            course.Course_name = jsonData.GetProperty("courseName").GetString();
            course.Academic_year = jsonData.GetProperty("academicYear").GetInt32();
            course.Course_description = jsonData.GetProperty("courseDescription").GetString();

            string saveCourseSql = "UPDATE course SET course_name = @Course_name, department_code = @Department_code, academic_year = @Academic_year, course_description = @Course_description" +
                 " WHERE course_code = Course_code;";

            int status = _data.SaveData(saveCourseSql, course, _config.GetConnectionString("Default"));

            if(status >= 0)
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
        [Authorize(Roles ="admin")]
        [HttpPatch("UpdateCoursePrerequisites")]
        public IActionResult UpdateCoursePrerequisities([FromBody] JsonElement jsonData)
        {
            //ADMIN ONLY.

            if (!jsonData.TryGetProperty("UserID", out JsonElement temp) || !CheckAdminExists(temp.GetInt32()))
            {

                return BadRequest(new { Message = "user id was not provided or is invalid." });
            }

            List<string> sqlList = new();
            List<dynamic> parameterList = new();

            if (!CheckUpdateCoursePrerequisitiesDataExist(jsonData))
            {

                return BadRequest(new { Message = "you have not sent all required data." });
            }


            string courseCode = jsonData.GetProperty("courseCode").GetString();

            List<string> prerequisites = new();

            foreach (JsonElement element in jsonData.GetProperty("prerequisites").EnumerateArray())
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
        [Authorize(Roles ="admin")]
        [HttpPatch("UpdateCourseDepartmentApplicability")]
        public IActionResult UpdateCourseDepartmentApplicability([FromBody] JsonElement jsonData)
        {
            //ADMIN ONLY.
            if (!jsonData.TryGetProperty("UserID", out JsonElement temp) || !CheckAdminExists(temp.GetInt32()))
            {

                return BadRequest(new { Message = "user id was not provided or is invalid." });
            }

            List<string> sqlList = new();
            List<dynamic> parameterList = new();

            if (!CheckUpdateDepartmentApplicabilityDataExist(jsonData))
            {
                return BadRequest(new { Message = "you have not sent all required data." });
            }


            string courseCode = jsonData.GetProperty("courseCode").GetString();

            List<string> departmentApplicability = new();

            List<string> availableDepartments = GetDepartmentsCodes();
            foreach (JsonElement element in jsonData.GetProperty("departmentApplicability").EnumerateArray())
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
        [HttpDelete("ArchiveCourse")]
        public IActionResult ArchiveCourse([FromBody] dynamic jsonData)
        {
            //ADMIN ONLY FUNCTION.
            if (!jsonData.TryGetProperty("UserID", out JsonElement temp) || !CheckAdminExists(temp.GetInt32()))
            {
                return BadRequest(new { Message = "user id is invalid." });
            }

            string courseCode;
            if (jsonData.TryGetProperty("CourseCode", out temp))
            {
                courseCode = temp.GetString();
            }
            else
            {
                return BadRequest(new { Message = "course code was not provided." });
            }

            List<Course> course = CheckCourseArchiveStatus(courseCode);

            if (course is not null && course.Count > 0 && !course.First().Is_archived)
            {
                string archiveCourseSql = "UPDATE course SET is_archived = TRUE WHERE course_code = @courseCode;";
                int status = _data.SaveData(archiveCourseSql, new { courseCode }, _config.GetConnectionString("Default"));
                if (status > 0)
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
            string getCourseInstancesByCodeSql = "SELECT * FROM course_instance WHERE course_code = @courseCode;";
            List<CourseInstance> courseInstances = _data.LoadData<CourseInstance, dynamic>(getCourseInstancesByCodeSql, new { courseCode }, _config.GetConnectionString("Default"));
            if (courseInstances is null)
            {

                return BadRequest(new { Message = "unknown error, maybe database server is down." });
            }

            else
            {

                return Ok(courseInstances);
            }

            //Returns all course instances matches the course code.
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
            string getCourseInstanceByIdSql = "SELECT * FROM course_instance WHERE instance_id = @instanceID;";
            List<CourseInstance> courseInstance = _data.LoadData<CourseInstance, dynamic>(getCourseInstanceByIdSql, new { instanceID }, _config.GetConnectionString("Default"));
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
        [Authorize(Roles ="admin")]
        [HttpPost("AddCourseInstance")]
        public IActionResult AddCourseInstance([FromBody] JsonElement jsonData)
        {
            //ADMIN ONLY FUNCTION.
            if (!jsonData.TryGetProperty("UserID", out JsonElement temp) || !CheckAdminExists(temp.GetInt32()))
            {
                return BadRequest(new { Message = "user id is invalid." });
            }

            if(!CheckAddCourseInstanceDataExist(jsonData))
            {
                return BadRequest(new { Message = "you have not sent all required data." });
            }

            CourseInstance courseInstance = new(jsonData);
            //Wait until we see how it's calculated or provided.Term();

            if(!CheckCourseExist(courseInstance.Course_code))
            {
                return BadRequest("course does not exist.");
            }

            if (courseInstance.Course_year == -1)
            {

                courseInstance.Course_year = TimeUtilities.GetCurrentYear();
            }

            string addCourseInstanceSql = "INSERT INTO course_instance VALUES(NULL, @Course_code, @Course_year, @Course_term, @Credit_hours);";

            int status = _data.SaveData(addCourseInstanceSql, courseInstance, _config.GetConnectionString("Default"));

            if(status > 0)
            {
                return Created("Department/AddCourseInstance", courseInstance);
            }
            else
            {
                return BadRequest(new { Message = "unknown error, maybe database server is down." });
            }

        }



        private static bool CheckUpdateCourseInfoExist(JsonElement jsonData)
        {
            return jsonData.TryGetProperty("courseCode", out _)
                && jsonData.TryGetProperty("academicYear", out _)
                && jsonData.TryGetProperty("courseName", out _)
                && jsonData.TryGetProperty("departmentCode", out _)
                && jsonData.TryGetProperty("courseDescription", out _);
        }

        private static bool CheckAddCourseInstanceDataExist(JsonElement jsonData)
        {
            return jsonData.TryGetProperty("Course_code", out _)
                && jsonData.TryGetProperty("Credit_hours", out _);
        }

        private static bool CheckUpdateCoursePrerequisitiesDataExist(JsonElement jsonData)
        {
            return jsonData.TryGetProperty("prerequisites", out _)
                && jsonData.TryGetProperty("courseCode", out _);
        }

        private static bool CheckUpdateDepartmentApplicabilityDataExist(JsonElement jsonData)
        {
            return jsonData.TryGetProperty("departmentApplicability", out _)
    && jsonData.TryGetProperty("courseCode", out _);
        }

        private bool CheckUserHasPriorities(int studentID)
        {
            string loadPrioritiesSql = "SELECT * FROM student_department_priority_list WHERE student_id = @studentID;";
            List<StudentDepartmentPriority> priorities = _data.LoadData<StudentDepartmentPriority, dynamic>(loadPrioritiesSql, new { studentID }, _config.GetConnectionString("Default"));

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
            return sentData.TryGetProperty("StudentID", out _)
            && sentData.TryGetProperty("DepartmentCode", out _);
        }

        private bool CheckCourseExist(string courseCode)
        {
            List<string> codes = _data.LoadData<string, dynamic>("SELECT course_code from course WHERE course_code = @courseCode;", new { courseCode }, _config.GetConnectionString("Default"));

            return codes != null && codes.Count > 0;
        }
        private static bool SetPriorityListRequiredDataExist(JsonElement sentData)
        {
            return sentData.TryGetProperty("StudentID", out _)
            && sentData.TryGetProperty("DepartmentCode1", out _)
            && sentData.TryGetProperty("DepartmentCode2", out _)
            && sentData.TryGetProperty("DepartmentCode3", out _)
            && sentData.TryGetProperty("DepartmentCode4", out _)
            && sentData.TryGetProperty("DepartmentCode5", out _);
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
        private bool CheckCourseData(JsonElement sentData)
        {


            return sentData.TryGetProperty("Course_code", out _)
                && sentData.TryGetProperty("departmentApplicability", out _)
                && (sentData.TryGetProperty("Department_code", out _) && GetDepartmentsCodes().Contains(sentData.GetProperty("Department_code").GetString()))
                && sentData.TryGetProperty("Course_name", out _);
        }

        private List<Course> CheckCourseArchiveStatus(string code)
        {
            string checkCourseSql = "SELECT course_code, is_archived FROM course  where course_code = @code";
            List<Course> courses = _data.LoadData<Course, dynamic>(checkCourseSql, new { code }, _config.GetConnectionString("Default"));

            return courses;
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
