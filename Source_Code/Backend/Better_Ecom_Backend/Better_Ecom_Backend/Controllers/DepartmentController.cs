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

            if(departments == null)
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
                return BadRequest(new { Message = "you have not sent all required data." });

            int studentID = jsonData.GetProperty("StudentID").GetInt32();
            List<string> sqlList = new();
            List<dynamic> parameterList = new();

            string sql = "SELECT * FROM student INNER JOIN system_user" + "\n"
            + "WHERE student.student_id = system_user.system_user_id" + "\n"
            + "AND system_user.system_user_id = @ID;";

            Student student = _data.LoadData<Student, dynamic>(sql, new { ID = studentID }, _config.GetConnectionString("Default")).FirstOrDefault();
            if (student == null)
            {
                return BadRequest(new { Message = "id does not exist or not a student." });
            }

            for (int i = 1; i <= 5; i++)
            {
                sqlList.Add($"INSERT INTO student_department_priority_list VALUES(@studentID, @department_code, @priority)");
                parameterList.Add(new { studentID, department_code = jsonData.GetProperty($"DepartmentCode{i}").GetString(), priority = i });
            }

            List<int> states = _data.SaveDataTransaction(sqlList, parameterList, _config.GetConnectionString("Default"));

            if(states.Contains(-1))
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
                return BadRequest(new { Message = "unknown error, maybe database server is down." });
            else if (rows.Count == 0)
                return Ok(new { Message = "student did not sumbit any priority list." });
            else
                return Ok(rows);
        }

        [Authorize(Roles ="admin")]
        [HttpPatch("SetDepartmentForStudent")]
        public IActionResult SetDepartmentForStudent([FromBody] dynamic inputData)
        {
            //ADMIN ONLY FUNCTION.
            JsonElement jsonData = (JsonElement)inputData;

            if (!SetDepartmentForStudentDataExist(jsonData))
                return BadRequest(new { Message = "some data is missing." });

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

                if(state > 0)
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

        /// <summary>
        /// This function takes Course object data and stores it in database.
        /// </summary>
        /// <param name="jsonData">a json object contains course data.</param>
        /// <returns>Created and course object if course is created BadRequest if failed</returns>
        [Authorize(Roles = "admin")]
        [HttpPost("AddCourseToDepartment")]
        public IActionResult AddCourseToDepartment([FromBody] dynamic jsonData)
        {
            //ADMIN ONLY FUNCTION.
            if (!jsonData.TryGetProperty("UserID", out JsonElement temp) || !CheckAdminExists(temp.GetInt32()))
                return BadRequest(new { Message = "user id was not provided or is invalid." });
            Course newCourse = new(jsonData);

            
            if (!CheckCourseData(newCourse))
            {
                return BadRequest(new { Message = "course data is not valid." });
            }

            //Insert course.
            string insertCourseSql = "INSERT INTO course(department_code, course_code, course_name, course_year, course_term, academic_year, course_description) " +
                "VALUES( @department_code, @course_code, @course_name, @course_year, @course_term, @academic_year, @course_description)";

            int status = _data.SaveData(insertCourseSql, newCourse, _config.GetConnectionString("Default"));

            if (status > 0)
            {
                return Created("/Department/AddCourse", newCourse);
            }
            else
                return BadRequest("unknown error, maybe database server is down.");

        }

        /// <summary>
        /// Archives a course.
        /// </summary>
        /// <param name="jsonData">json object containing the course id admin with to archive.</param>
        /// <returns></returns>
        [Authorize(Roles ="admin")]
        [HttpDelete("ArchiveCourse")]
        public IActionResult ArchiveCourse([FromBody] dynamic jsonData)
        {
            //ADMIN ONLY FUNCTION.
            int courseID;
            if (!jsonData.TryGetProperty("UserID", out JsonElement temp) || !CheckAdminExists(temp.GetInt32()))
                return BadRequest(new { Message = "user id is invalid." });
      
            if (jsonData.TryGetProperty("CourseID", out temp))
            {
                courseID = temp.GetInt32();
            }
            else
            {
                return BadRequest(new { Message = "course id was not provided." });
            }

            List<Course> course = CheckCourseStatus(courseID);



            if (course is not null && course.Count > 0 && !course.First().Is_archived)
            {
                string archiveCourseSql = "UPDATE course SET is_archived = 1 WHERE course_id = @courseID;";
                int status = _data.SaveData(archiveCourseSql, new { courseID }, _config.GetConnectionString("Default"));
                if (status > 0)
                    return Ok(new { Message = "course archived." });
                else
                    return BadRequest(new { Message = "unknown error, maybe database server is down." });
            }
            else {
                if (course is null)
                    return BadRequest(new { Message = "unknown error, maybe database server is down." });
                else if (course.Count == 0)
                    return BadRequest(new { Message = "course does not exist." });
                else if (course.First().Is_archived)
                    return BadRequest(new { Message = "course already archived." });
                
                
            }
  
            return BadRequest();

        }

        private static bool SetDepartmentForStudentDataExist(JsonElement sentData)
        {
            return sentData.TryGetProperty("StudentID", out _)
            && sentData.TryGetProperty("DepartmentCode", out _);
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
                return false;
            else
                return true;

        }
        private bool CheckCourseData(Course course)
        {
            return course.Academic_year > 0
                && (course.Department_code is not null && GetDepartmentsCodes().Contains(course.Department_code))
                && course.Course_code is not null
                && course.Course_name is not null
                && course.Course_year > 0
                && course.Is_archived is false;
        }

        private List<Course> CheckCourseStatus(int ID)
        {
            string checkCourseSql = "SELECT course_id, is_archived FROM course  where course_id = @ID";

            List<Course> courses = _data.LoadData<Course, dynamic>(checkCourseSql, new { ID }, _config.GetConnectionString("Default"));

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
