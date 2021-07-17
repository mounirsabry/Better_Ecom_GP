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

        [Authorize]
        [HttpPost("ChooseDepartments")]
        public IActionResult ChooseDepartments([FromBody] dynamic inputData)
        {
            //STUDENT ONLY FUNCTION.
            //The student can use it to enter the priority list for the first time, then we should insert the priority list
            //into the database.
            //The student can use to overwrite the old choices, then we can delete the old records and insert or update the old records.
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
            // to departmentCode5, departmentCode1 represents the highest while departmentCode5 represent the lowest.
            //Pack those variables into a list of student department priority class.
            //Save to database.
            //Return message successful, other option return a list of those options.
        }

        [Authorize]
        [HttpGet("GetStudentPriorityList/{ID:int}")]
        public IActionResult GetStudentPriorityList(int id)
        {
            //ADMIN, STUDENT FUNCTION.
            //Admin enters any student ID, get the priority list.
            //Student enters his ID, get the priority list that he entered.
            //Get the list from the database and return it.

            string sql = "SELECT department_code, priority FROM student_department_priority_list WHERE student_id = @id;";

            dynamic rows = _data.LoadData<dynamic, int>(sql, id, _config.GetConnectionString("Default"));

            if (rows == null)
                return BadRequest(new { Message = "unknown error, maybe database server is down." });
            else if (rows.Count == 0)
                return Ok(new { Message = "student did not sumbit any priority list." });
            else
                return Ok(rows);
        }

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

        [HttpPost("AddCourse")]
        public IActionResult AddCourse([FromBody] dynamic jsonData)
        {
            //ADMIN ONLY FUNCTION.
            int userID;
            if (jsonData.TryGetProperty("UserID", out JsonElement temp))
            {
                userID = temp.GetInt32();
            }
            else
            {
                return BadRequest(new { Message = "user id was not provided." });
            }
            Course newCourse = new(jsonData);

            //Check the given id against the admin table, if not valid
            //then the function should return a bad request saying that the user id entered does not exist.

            //Check the entered data.
            //Maybe check the course id, if provided, then refuse, or don't check.
            //Department code can be null, but if provided, it should be a valid code from the database.
            //Course code, course name should not be null.
            //Course year should not be -1 or any value less than zero.
            //Do not check the course term, academic year, course description.
            //Is_archived must be false.
            if (!CheckCourseData(newCourse))
            {
                return BadRequest(new { Message = "course data is not valid." });
            }

            //Insert course.

            //Return the inserted course, should return the course with the updated course id from the database.
            return Ok(new { Message = "not implemented yet." });
        }

        [HttpDelete("ArchiveCourse")]
        public IActionResult ArchiveCourse([FromBody] dynamic jsonData)
        {
            //ADMIN ONLY FUNCTION.
            JsonElement temp;
            int userID;
            int courseID;
            if (jsonData.TryGetProperty("UserID", out temp))
            {
                userID = temp.GetInt32();
            }
            else
            {
                return BadRequest(new { Message = "user id was not provided." });
            }
            if (jsonData.TryGetProperty("CourseID", out temp))
            {
                courseID = temp.GetInt32();
            }
            else
            {
                return BadRequest(new { Message = "course id was not provided." });
            }
            //Check the given id against the admin table, if not valid
            //then the function should return a bad request saying that the user id entered does not exist.

            //Archive the course by setting the value of is_archive to true in the database.


            //IF course is archived, return a message :the course is archived.
            //IF the course is already archived, return a message :the course is already archived.
            //If course was not successfully archived, return a bad request with a message :the course id was not found.
            return Ok(new { Messgae = "not implemented yet." });
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

        private static bool CheckCourseData(Course course)
        {
            return true;
        }
    }
}
