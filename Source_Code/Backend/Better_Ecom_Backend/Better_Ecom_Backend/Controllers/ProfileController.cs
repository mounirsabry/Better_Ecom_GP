using Better_Ecom_Backend.Helpers;
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
    public class ProfileController : ControllerBase
    {
        private IConfiguration _config;
        private IDataAccess _data;

        public ProfileController(IConfiguration config, IDataAccess data)
        {
            _config = config;
            _data = data;
        }

        [Authorize]
        [HttpGet("GetProfile/{ID:int}/{Type}")]
        public dynamic GetProfile(int id, string type)
        {
            string id_text = "";
            string table = "";
            switch (type)
            {
                case "student":
                    table = "student";
                    id_text = "student.student_id";
                    break;
                case "instructor":
                    table = "instructor";
                    id_text = "instructor.instructor_id";
                    break;
                case "admin":
                    table = "admin_user";
                    id_text = "admin_user.admin_user_id";
                    break;
                default:
                    table = "invalid";
                    id_text = "invalid";
                    break;
            }

            if (table == "invalid")
            {
                return BadRequest(new { Message = "invalid user type." });
            }
            string sql = @$"SELECT * FROM {table} INNER JOIN system_user
                    WHERE {id_text} = system_user.system_user_id 
                    AND system_user.system_user_id = @ID;";

            return _data.LoadData<dynamic, dynamic>(sql, new { ID = id }, _config.GetConnectionString(Constants.CurrentDBConnectionStringName)).FirstOrDefault();
        }

        // not sure if the admin will use this to modify student/instructor profiles, will assume not till i get there.
        [Authorize]
        [HttpPatch("SaveProfileChanges/{ID:int}/{Type}")]
        public IActionResult SaveProfileChanges(int id, string type, [FromBody] dynamic data)
        {
            int success1 = 0;
            int success2 = 0;
            if (type != "student" && type != "instructor" && type != "admin")
            {
                return BadRequest(new { Message = "invalid user type." });
            }
            else
            {
                System_user system_user = UserFactory.getUser(data, type);
                List<string> queries = new List<string>();
                queries.Add(GetBaseUserUpdateQuery());
                if (type == "instructor")
                    queries.Add(GetInstructorUpdateQuery());

                try
                {

                    success1 = _data.SaveDataTransaction<System_user>(queries, system_user, _config.GetConnectionString(Constants.CurrentDBConnectionStringName));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return BadRequest(new { Message = "operation failed." });
                }
            }
            if (success1 > 0)
            {
                return Ok();
            }
            else
            {
                return BadRequest(new { Message = "operation failed." });
            }
        }

        private string GetInstructorUpdateQuery()
        {
            return "UPDATE instructor SET contact_info = @Contact_info where instructor_id = @Instructor_id;";
        }

        private string GetBaseUserUpdateQuery()
        {
            return @$"UPDATE system_user SET email = @Email, address = @Address, phone_number = @Phone_number, mobile_number = @Mobile_number,
                additional_info = @Additional_info  where system_user_id = @System_user_id;";
        }

        [Authorize]
        [HttpPatch("ChangePassword/{ID:int}")]
        public IActionResult ChangePassword(int id, [FromBody] dynamic data)
        {
            data = (JsonElement)data;
            string sql = "SELECT user_password FROM system_user WHERE system_user_id = @ID;";
            int success = 0;
            string current_password = _data.LoadData<string, dynamic>(sql, new { ID = id }, _config.GetConnectionString(Constants.CurrentDBConnectionStringName))[0];
            string sent_current_password = data.GetProperty("Old_password").GetString();
            string new_password = data.GetProperty("New_password").GetString();

            if (current_password != sent_current_password)
            {
                return BadRequest("old password is wrong.");
            }
            else
            {
                sql = "UPDATE system_user SET user_password = @new_password WHERE system_user_id = @ID;";

                try
                {
                    success = _data.SaveData<dynamic>(sql, new { ID = id, new_password = new_password }, _config.GetConnectionString(Constants.CurrentDBConnectionStringName));
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                    return BadRequest(new { Message = "operation failed." });
                }

                if (success > 0)
                {
                    return Ok();
                }
                else
                {
                    return BadRequest(new { Message = "password update failed." });
                }
            }
        }
    }
}
