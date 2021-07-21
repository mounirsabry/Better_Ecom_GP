using Better_Ecom_Backend.Helpers;
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
        /// <param name="userID">the user id.</param>
        /// <param name="type">the user type</param>
        /// <returns>Ok with user profile if successful BadRequest otherwise.</returns>
        [Authorize]
        [HttpGet("GetProfile/{UserID:int}/{Type}")]
        public dynamic GetProfile([FromHeader] string Authorization , int userID, string type)
        {
            TokenInfo idAndType = HelperFunctions.getIdAndTypeFromToken(Authorization);

            string deducedType = HelperFunctions.GetUserTypeFromID(userID);
            if (type != deducedType || deducedType == "invalid")
            {
                return BadRequest(new { Message = "invalid id." });
            }
            else if (type != "student" && type != "instructor" && type != "admin")
            {
                return BadRequest(new { Message = "invalid user type." });
            }
            else if (idAndType.Type == "student" && idAndType.UserID != userID)
            {
                return Forbid("students can only get their own info.");
            }

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
                return BadRequest(new { Message = "unknown error, maybe database server is down." });
            }
            else if (dbResult.Count == 0)
            {
                return BadRequest(new { Messsage = "no user found with such id." });
            }
            else
            {
                var user = dbResult[0];
                user.user_password = null;
                return user;
            }
        }

        // not sure if the admin will use this to modify student/instructor profiles, will assume not till i get there.
        [Authorize]
        [HttpPatch("SaveProfileChanges")]
        public IActionResult SaveProfileChanges([FromBody] JsonElement data)
        {
            int userID;
            string type;


            if(!SaveProfileChangesRequiredDataValid(data))
            {
                return BadRequest(new { Message = "required data missing or invalid." });
            }

            if (data.TryGetProperty("UserID", out JsonElement temp) && temp.TryGetInt32(out _))
            {
                userID = temp.GetInt32();
            }
            else
            {
                return BadRequest(new { Message = "user id was not provided." });
            }

            if (data.TryGetProperty("Type", out temp) && temp.ValueKind == JsonValueKind.String)
            {
                type = temp.GetString();
            }
            else
            {
                return BadRequest(new { Message = "user type was not provided." });
            }

            string deducedType = HelperFunctions.GetUserTypeFromID(userID);
            if (type != deducedType || deducedType == "invalid")
            {
                return BadRequest(new { Message = "invalid id." });
            }
            else if (type != "student" && type != "instructor" && type != "admin")
            {
                return BadRequest(new { Message = "invalid user type." });
            }

            int System_user_id = userID;
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

            List<int> success;
            success = _data.SaveDataTransaction<dynamic>(queries, parameterList, _config.GetConnectionString("Default"));

            if (!success.Contains(-1))
            {
                return Ok();
            }
            else
            {
                return BadRequest(new { Message = "unknown error, maybe database is down." });
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
        public IActionResult ChangePassword([FromBody] dynamic data)
        {
            data = (JsonElement)data;

            if (!ChangePasswordRequiredDataValid(data))
            {
                return BadRequest(new { Message = "required data missing or invalid." });
            }

            int userID = data.GetProperty("UserID").GetInt32();
            string sent_current_password = data.GetProperty("Old_password").GetString();
            string new_password = data.GetProperty("New_password").GetString();
            string current_password;

            if (HelperFunctions.GetUserTypeFromID(userID) == "invalid")
            {
                return BadRequest(new { Message = "invalid id." });
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
                return BadRequest(new { Message = "unknown error, maybe database server is down." });
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
            return sentData.TryGetProperty("UserID", out JsonElement temp) && temp.TryGetInt32(out _)
                && sentData.TryGetProperty("Old_password", out temp) && temp.ValueKind == JsonValueKind.String
                && sentData.TryGetProperty("New_password", out temp) && temp.ValueKind == JsonValueKind.String;
        }

        private static bool SaveProfileChangesRequiredDataValid(JsonElement data)
        {
            return data.TryGetProperty("UserID", out JsonElement temp) && temp.TryGetInt32(out _)
                && data.TryGetProperty("Type", out temp) && temp.ValueKind == JsonValueKind.String;
        }
    }
}
