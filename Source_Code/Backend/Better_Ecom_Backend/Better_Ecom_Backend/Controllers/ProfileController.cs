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
            // see temporary tables in sql.
            string sql = "";
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
            }
            sql = @$"SELECT * FROM {table} INNER JOIN system_user
                    WHERE {id_text} = system_user.system_user_id 
                    AND system_user.system_user_id = @ID";
            return _data.LoadData<dynamic, dynamic>(sql, new { ID = id }, _config.GetConnectionString(Constants.CurrentDBConnectionStringName)).FirstOrDefault();
        }

        // not sure if the admin will use this to modify student/instructor profiles, will assume not till i get there.
        [Authorize]
        [HttpPatch("SaveProfileChanges/{ID:int}/{Type}")]
        public IActionResult SaveProfileChanges(int id, string type, [FromBody] dynamic data)
        {
            int success;
            if (type != "admin" && type != "student" && type != "instructor")
            {

                return BadRequest();

            }
            else
            {
                System_user system_user = UserFactory.getUser(data, type);

                success = _data.SaveData<System_user>(GetBaseUserUpdateQuery(), system_user, _config.GetConnectionString(Constants.CurrentDBConnectionStringName));
                if(type == "instructor")
                    success += _data.SaveData<System_user>(GetInstructorUpdateQuery(), system_user, _config.GetConnectionString(Constants.CurrentDBConnectionStringName));
            }
            if (success > 0)
                return Ok();
            else
                return BadRequest();
        }

        private string GetInstructorUpdateQuery()
        {

             
                return "contact_info = @Contact_info where instructor_id = @Instructor_id";

        }

        private string GetBaseUserUpdateQuery()
        {
            return @$"UPDATE system_user SET email = 'g.r33r@hotmail.com', address = @Address, phone_number = @Phone_number, mobile_number = @Mobile_number,
                additional_info = @Additional_info  where system_user_id = @System_user_id";
        }

        [Authorize]
        [HttpPatch("ChangePassword/{ID:int}")]
        public IActionResult ChangePassword(int id, [FromBody] dynamic data)
        {
            data = (JsonElement)data;
            string sql = "SELECT user_password FROM system_user WHERE system_user_id = @ID";

            string current_password = _data.LoadData<string, dynamic>(sql, new { ID = id }, _config.GetConnectionString(Constants.CurrentDBConnectionStringName))[0];
            string sent_current_password = data.GetProperty("old_password").GetString();
            string new_password = data.GetProperty("new_password").GetString();

            if (current_password != sent_current_password)
            {
                return BadRequest("OLD PASSWORD IS WRONG");
            }
            else
            {
                sql = "UPDATE system_user SET user_password = @new_password WHERE system_user_id = @ID";

                int success = _data.SaveData<dynamic>(sql, new { ID = id, new_password = new_password }, _config.GetConnectionString(Constants.CurrentDBConnectionStringName));

                if (success > 0)
                {
                    return Ok();
                }
                else
                {
                    return BadRequest();
                }
            }
        }
    }
}
