using Better_Ecom_Backend.Entities;
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

        /// <summary>
        /// Get user profile.
        /// </summary>
        /// <returns>Ok with user profile if successful BadRequest otherwise.</returns>
        [Authorize]
        [HttpGet("GetProfile")]
        public dynamic GetProfile([FromHeader] string Authorization)
        {
            //ALL USERS FUNCTION.
            //Each user gets his own profile.
            TokenInfo tokenInfo = HelperFunctions.GetIdAndTypeFromToken(Authorization);
            int userID = tokenInfo.UserID;
            string type = tokenInfo.Type;

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
            var dbResult = _data.LoadData<dynamic, dynamic>(sql, new { ID = userID }, _config.GetConnectionString("Default"));

            if (dbResult == null)
            {
                return BadRequest(new { Message = MessageFunctions.GetMaybeDatabaseIsDownMessage() });
            }
            else if (dbResult.Count == 0)
            {
                return BadRequest(new { Messsage = "no user found with such id." });
            }
            else
            {
                var user = dbResult[0];
                user.user_password = null;
                return Ok(user);
            }
        }

        [Authorize]
        [HttpPatch("SaveProfileChanges")]
        public IActionResult SaveProfileChanges([FromHeader] string Authorization, [FromBody] JsonElement data)
        {
            //ALL USERS FUNCTION.
            //Each user updates his own data.
            TokenInfo tokenInfo = HelperFunctions.GetIdAndTypeFromToken(Authorization);
            int userID = tokenInfo.UserID;
            string type = tokenInfo.Type;

            if (SaveProfileChangesRequiredDataValid(data) == false)
            {
                return BadRequest(new { Message = "required data missing or invalid." });
            }
            else if (type != "student" && type != "instructor" && type != "admin")
            {
                return BadRequest(new { Message = "invalid user type." });
            }
            else if (type == "instructor")
            {
                bool isContactInfoSent = data.TryGetProperty(nameof(Instructor.Contact_info), out JsonElement temp) && temp.ValueKind == JsonValueKind.String;
                if (isContactInfoSent == false)
                {
                    return BadRequest(new { Message = "instructor contact info was not provided or was not a string." });
                }
            }

            System_user system_user = UserFactory.getUser(data, type);
            system_user.System_user_id = userID;

            List<string> queries = new();
            List<dynamic> parameterList = new();
            queries.Add(GetBaseUserUpdateQuery());
            parameterList.Add(new
            {
                system_user.System_user_id,
                system_user.Email,
                system_user.Address,
                system_user.Phone_number,
                system_user.Mobile_number,
                system_user.Additional_info
            });
            if (type == "instructor")
            {
                queries.Add(GetInstructorUpdateQuery());
                parameterList.Add(new { Instructor_id = system_user.System_user_id, ((Instructor)system_user).Contact_info });
            }

            List<int> success;
            success = _data.SaveDataTransaction<dynamic>(queries, parameterList, _config.GetConnectionString("Default"));

            if (!success.Contains(-1))
            {
                return Ok(new { Message = "user data was updated successfully." });
            }
            else
            {
                return BadRequest(new { Message = MessageFunctions.GetMaybeDatabaseIsDownMessage() });
            }
        }

        /// <summary>
        /// User wants to change password.
        /// </summary>
        /// <param name="id">user id</param>
        /// <param name="data">json object contains current password and new password.</param>
        /// <returns>Ok if successful BadRequest otherwise.</returns>
        [Authorize]
        [HttpPatch("ChangePassword")]
        public IActionResult ChangePassword([FromHeader] string Authorization, [FromBody] dynamic data)
        {
            data = (JsonElement)data;
            TokenInfo tokenInfo = HelperFunctions.GetIdAndTypeFromToken(Authorization);

            if (!ChangePasswordRequiredDataValid(data))
            {
                return BadRequest(new { Message = "required data missing or invalid." });
            }

            int userID = tokenInfo.UserID;
            string sent_current_password = data.GetProperty("OldPassword").GetString();
            string new_password = data.GetProperty("NewPassword").GetString();
            string current_password;

            if (HelperFunctions.GetUserTypeFromID(userID) == "invalid")
            {
                return BadRequest(new { Message = "invalid id or type." });
            }
            else if (sent_current_password == new_password)
            {
                return BadRequest(new { Message = "new password can not be the same as old password." });
            }

            string sql = "SELECT user_password FROM system_user WHERE system_user_id = @ID;";
            var dbResult = _data.LoadData<string, dynamic>(sql, new { ID = userID }, _config.GetConnectionString("Default"));
            if (dbResult != null)
            {
                current_password = dbResult.FirstOrDefault();
            }
            else
            {
                return BadRequest(new { Message = MessageFunctions.GetMaybeDatabaseIsDownMessage() });
            }

            if (!SecurityUtilities.Verify(sent_current_password, current_password))
            {
                return BadRequest(new { Message = "old password is wrong." });
            }
            else
            {
                sql = "UPDATE system_user SET user_password = @new_password WHERE system_user_id = @ID;";
                int success = _data.SaveData<dynamic>(sql, new { ID = userID, new_password = SecurityUtilities.HashPassword(new_password) }, _config.GetConnectionString("Default"));

                if (success >= 0)
                {
                    return Ok(new { Message = "password was changed successfully." });
                }
                else
                {
                    return BadRequest(new { Message = "password update failed." });
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

        private static bool ChangePasswordRequiredDataValid(JsonElement sentData)
        {
            return sentData.TryGetProperty("OldPassword", out JsonElement temp) && temp.ValueKind == JsonValueKind.String
                && sentData.TryGetProperty("NewPassword", out temp) && temp.ValueKind == JsonValueKind.String;
        }

        private static bool SaveProfileChangesRequiredDataValid(JsonElement data)
        {
            return data.TryGetProperty(nameof(System_user.Email), out JsonElement temp) && temp.ValueKind == JsonValueKind.String
                && data.TryGetProperty(nameof(System_user.Phone_number), out temp) && temp.ValueKind == JsonValueKind.String
                && data.TryGetProperty(nameof(System_user.Mobile_number), out temp) && temp.ValueKind == JsonValueKind.String
                && data.TryGetProperty(nameof(System_user.Additional_info), out temp) && temp.ValueKind == JsonValueKind.String;
        }
    }
}
