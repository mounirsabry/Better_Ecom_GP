﻿using Better_Ecom_Backend.Models;
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
        private IConfiguration _config;
        private IDataAccess _data;

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

            //Fetch all the departments from the database.
            //And return them, department code and name both.

    
        }

        [Authorize]
        [HttpPost("ChooseDepartments")]
        public IActionResult ChooseDepartments([FromBody] dynamic inputData)
        {
            JsonElement jsonData = (JsonElement)inputData;

            if (!SetPriorityListRequiredDataExist(jsonData))
                return BadRequest(new { Message = "you haven't sent all requred data." });

            int studentID = jsonData.GetProperty("studentID").GetInt32();
            List<string> sqlList = new List<string>();
            List<dynamic> parameterList = new List<dynamic>();

            for (int i = 1; i <= 5; i++)
            {
                sqlList.Add($"INSERT INTO student_department_priority_list VALUES(@studentID, @department_code, @priority)");
                parameterList.Add(new { studentID = studentID, department_code = jsonData.GetProperty($"DepartmentCode{i}").GetString(), priority = i });
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
            //ADMIN ONLY FUNCTION.
            //Get the list from the database and return it.

            string sql = "SELECT department_code, priority FROM student_department_priority_list WHERE student_id = @id ";

            dynamic rows = _data.LoadData<dynamic, int>(sql, id, _config.GetConnectionString("Defauld"));

            if (rows == null)
                return BadRequest(new { Message = "operation failed." });
            else if (rows.Count == 0)
                return Ok(new { Message = "student haven't submitted any priority list." });
            else
                return Ok(rows);
        }

        [HttpPatch("SetDepartmentForStudent")]
        public IActionResult SetDepartmentForStudent([FromBody] dynamic inputData)
        {
            //ADMIN ONLY FUNCTION.
            JsonElement jsonData = (JsonElement)inputData;

            if (!SetDepartmentForStudentDataExist(jsonData))
                return BadRequest(new { Message = "you haven't sent all requred data." });

            int studentID = jsonData.GetProperty("StudentID").GetInt32();
            string departmentCode = jsonData.GetProperty("DepartmentCode").GetString();

            string sql = "SELECT * FROM student INNER JOIN system_user" + "\n"
                    + "WHERE student.student_id = system_user.system_user_id" + "\n"
                    + "AND system_user.system_user_id = @ID;";

            Student student = _data.LoadData<Student, dynamic>(sql, new { ID = studentID }, _config.GetConnectionString("Default")).FirstOrDefault();

            if (student != null)
            {
                string studentUpdateSql = "UPDATE student SET department_code = @Department_code";
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
                return BadRequest(new { Message = "id doesn't exist." });
            }

            //update query for table student.
            //Optional, return the student object.

        }


        private bool SetDepartmentForStudentDataExist(JsonElement sentData)
        {
            JsonElement temp;
            return sentData.TryGetProperty("StudentID", out temp)
            && sentData.TryGetProperty("DepartmentCode", out temp);
        }

        private bool SetPriorityListRequiredDataExist(JsonElement sentData)
        {
            JsonElement temp;
            return sentData.TryGetProperty("StudentID", out temp)
            && sentData.TryGetProperty("DepartmentCode1", out temp)
            && sentData.TryGetProperty("DepartmentCode2", out temp)
            && sentData.TryGetProperty("DepartmentCode3", out temp)
            && sentData.TryGetProperty("DepartmentCode4", out temp)
            && sentData.TryGetProperty("DepartmentCode5", out temp);

        }
    }
}
