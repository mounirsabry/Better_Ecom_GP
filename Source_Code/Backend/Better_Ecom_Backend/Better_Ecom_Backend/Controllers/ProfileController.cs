using Better_Ecom_Backend.Helpers;
using Better_Ecom_Backend.Models;
using DataLibrary;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace Better_Ecom_Backend.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IDataAccess _data;

        public ProfileController(IConfiguration config, IDataAccess data)
        {
            _config = config;
            _data = data;
        }

        [Authorize]
        [HttpGet("GetProfile/{ID:int}/{Type}")]
        public dynamic GetProfile(int id, string type)
        {
            string id_text;
            string table;
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
                    return BadRequest(new { Message = "invalid user type." });
            }
            string sql = $"SELECT * FROM {table} INNER JOIN system_user" + "\n"
                    + $"WHERE {id_text} = system_user.system_user_id" + "\n"
                    + "AND system_user.system_user_id = @ID;";
            var dbResult = _data.LoadData<dynamic, dynamic>(sql, new { ID = id }, _config.GetConnectionString("Default"));

            if (dbResult != null)
            {

                var user = dbResult.FirstOrDefault();
                if(user is not null)
                    user.user_password = null;
                return user;
            }
            else
                return BadRequest(new { Message = "unknown error, maybe database server is down." });
        }

        // not sure if the admin will use this to modify student/instructor profiles, will assume not till i get there.
        [Authorize]
        [HttpPatch("SaveProfileChanges/{ID:int}/{Type}")]
        public IActionResult SaveProfileChanges(int ID, string type, [FromBody] dynamic data)
        {
            List<int> success1;
            if (type != "student" && type != "instructor" && type != "admin")
            {
                return BadRequest(new { Message = "invalid user type." });
            }
            else
            {
                int System_user_id = ID;
                System_user system_user = UserFactory.getUser(data, type);
                List<string> queries = new();
                List<dynamic> parameterList = new();
                queries.Add(GetBaseUserUpdateQuery());
                parameterList.Add(new
                {
                    System_user_id,
                    system_user.Email,
                    system_user.Address,
                    system_user.Phone_number,
                    system_user.Mobile_number,
                    system_user.Additional_info
                });
                if (type == "instructor")
                {
                    queries.Add(GetInstructorUpdateQuery());
                    parameterList.Add(new { ((Instructor)system_user).Contact_info });
                }

                success1 = _data.SaveDataTransaction<dynamic>(queries, parameterList, _config.GetConnectionString("Default"));

                if (!success1.Contains(-1))
                {
                    return Ok();
                }
                else
                {
                    return BadRequest(new { Message = "operation failed." });
                }
            }

        }

        private static string GetInstructorUpdateQuery()
        {
            return "UPDATE instructor SET contact_info = @Contact_info where instructor_id = @Instructor_id;";
        }

        private static string GetBaseUserUpdateQuery()
        {
            return "UPDATE system_user SET email = @Email, address = @Address, phone_number = @Phone_number, mobile_number = @Mobile_number," + "\n"
                + "additional_info = @Additional_info  where system_user_id = @System_user_id;";
        }

        [Authorize]
        [HttpPatch("ChangePassword/{ID:int}")]
        public IActionResult ChangePassword(int id, [FromBody] dynamic data)
        {
            data = (JsonElement)data;
            int success;

            if (!ChangePasswordDataExist(data))
                return BadRequest(new { Message = "sent data not complete." });

            string sent_current_password = data.GetProperty("Old_password").GetString();
            string new_password = data.GetProperty("New_password").GetString();
            string current_password;


            string sql = "SELECT user_password FROM system_user WHERE system_user_id = @ID;";
            var dbResult = _data.LoadData<string, dynamic>(sql, new { ID = id }, _config.GetConnectionString("Default"));
            if (dbResult != null)
                current_password = dbResult.FirstOrDefault();
            else
                return BadRequest(new { Message = "unknown error, maybe database server is down." });


            if (!SecurityUtilities.Verify(  sent_current_password,current_password ))
            {
                return BadRequest("old password is wrong.");
            }
            else
            {
                sql = "UPDATE system_user SET user_password = @new_password WHERE system_user_id = @ID;";
                success = _data.SaveData<dynamic>(sql, new { ID = id, new_password = SecurityUtilities.HashPassword( new_password) }, _config.GetConnectionString("Default"));

                if (success >= 0)
                {
                    return Ok();
                }
                else
                {
                    return BadRequest(new { Message = "password update failed." });
                }
            }
        }

#pragma warning disable CA1822 // Mark members as static
        private bool ChangePasswordDataExist(JsonElement sentData)
#pragma warning restore CA1822 // Mark members as static
        {
            return sentData.TryGetProperty("Old_password", out _)
                && sentData.TryGetProperty("New_password", out _);
        }
    }
}
