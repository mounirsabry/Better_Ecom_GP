using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        [HttpGet("GetDepartments")]
        public IActionResult GetDepartments()
        {
            //Fetch all the departments from the database.
            //And return them, department code and name both.

            return Ok("not implemented yet.");
        }

        [HttpPost("ChooseDepartments")]
        public IActionResult ChooseDepartments([FromBody] dynamic inputData)
        {
            JsonElement jsonData = (JsonElement)inputData;
            int studnetID = jsonData.GetProperty("StudentID").GetInt32();
            string departmentCode1 = jsonData.GetProperty("DepartmentCode1").GetString();
            // to departmentCode5, departmentCode1 represents the highest while departmentCode5 represent the lowest.

            //Pack those variables into a list of student department priority class.

            //Save to database.

            //Return message successful, other option return a list of those options.

            return Ok("not implemented yet.");
        }

        [HttpGet("GetStudentPriorityList/{ID:int}")]
        public IActionResult GetStudentPriorityList(int id)
        {
            //ADMIN ONLY FUNCTION.
            //Get the list from the database and return it.

            return Ok("not implemented yet.");
        }

        [HttpPatch("SetDepartmentForStudent")]
        public IActionResult SetDepartmentForStudent([FromBody] dynamic inputData)
        {
            //ADMIN ONLY FUNCTION.
            JsonElement jsonData = (JsonElement)inputData;
            int studentID = jsonData.GetProperty("StudentID").GetInt32();
            string departmentCode = jsonData.GetProperty("DepartmentCode").GetString();

            //update query for table student.
            //Optional, return the student object.
            return Ok("not implemented yet.");
        }
    }
}
