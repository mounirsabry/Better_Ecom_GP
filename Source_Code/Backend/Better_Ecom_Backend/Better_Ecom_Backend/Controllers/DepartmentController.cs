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
    [Route("api/[controller]")]
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
            JsonElement jsonData = (JsonElement)inputData;

            if (!SetPriorityListRequiredDataExist(jsonData))
                return BadRequest(new { Message = "you have not sent all required data." });

            int studentID = jsonData.GetProperty("StudentID").GetInt32();
            List<string> sqlList = new();
            List<dynamic> parameterList = new();

            string sql = "SELECT student_id FROM student WHERE student.student_id = @ID;";

            List<int> student = _data.LoadData<int, dynamic>(sql, new { ID = studentID }, _config.GetConnectionString("Default"));
            if (student == null)
                return BadRequest(new { Message = "unknown error, maybe database server is down." });
            else if (student.Count == 0)
                return BadRequest(new { Message = "id does not exist or not a student." });
            

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

            string sql = "SELECT student_id, department_code, priority FROM student_department_priority_list WHERE student_id = @id;";

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
            Student student;

            string sql = "SELECT student_id FROM student WHERE student.student_id = @ID";

            var dbResult = _data.LoadData<Student, dynamic>(sql, new { ID = studentID }, _config.GetConnectionString("Default"));

            if (dbResult != null)
                student = dbResult.FirstOrDefault();
            else
                return BadRequest(new { Message = "unknown error, maybe database server is down." });

            student.Department_code = departmentCode;


             

            if (student != null)
            {
                string studentUpdateSql = "UPDATE student SET department_code = @Department_code WHERE student_id = @Student_id;";
                int state = _data.SaveData(studentUpdateSql, student, _config.GetConnectionString("Default"));

                if(state > 0)
                {
                    return Ok();
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
    }
}
