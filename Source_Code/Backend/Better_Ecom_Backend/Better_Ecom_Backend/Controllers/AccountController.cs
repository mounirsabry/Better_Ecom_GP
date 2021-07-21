using Better_Ecom_Backend.Helpers;
using Better_Ecom_Backend.Models;
using DataLibrary;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Better_Ecom_Backend.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IDataAccess _data;

        public AccountController(IConfiguration config, IDataAccess data)
        {
            _config = config;
            _data = data;
        }

        /// <summary>
        /// The client provides id, password and type that is checked if they are valid the client is authorized to use the api.
        /// </summary>
        /// <param name="loginData">json object contains id, password and type.</param>
        /// <returns>authorization token if provided data is valid BadRequest otherwise.</returns>
        [HttpPost("Login")]
        public IActionResult Login([FromBody] JsonElement loginData)
        {

            if(!LoginRequiredDataValid(loginData))
            {
                return BadRequest(new { Message = "required data missing or invalid." });
            }

            int userID = loginData.GetProperty("UserID").GetInt32();
            string password = loginData.GetProperty("Password").GetString();
            string type = loginData.GetProperty("Type").GetString();

            if (!HelperFunctions.CheckUserIDAndType(userID, type))
            {
                return BadRequest(new { Message = "invalid user id or type." });
            }

            //IActionResult
            IActionResult response = Unauthorized();
            bool exists = AuthenticateUser(userID, password, type) && !string.IsNullOrEmpty(password);

            if (exists)
            {
                string tokenString = GenerateJSONWebToken(userID, type);
                response = Ok(new { token = tokenString });
            }
            return response;
        }

        /// <summary>
        /// This function creates account for already existing student.
        /// </summary>
        /// <param name="inputData">json object contains required data.</param>
        /// <returns></returns>
        [Authorize(Roles = "admin")]
        [HttpPost("CreateAccountForStudent")]
        public IActionResult CreateAccountForStudent([FromBody] dynamic inputData)
        {
            JsonElement inputJson = (JsonElement)inputData;

            if (!CreateAccountForStudentDataExist(inputJson))
            {
                return BadRequest(new { Message = "sent data is not complete." });
            }

            int studentID = inputJson.GetProperty("StudentID").GetInt32();
            if (HelperFunctions.GetUserTypeFromID(studentID) != "student")
            {
                return BadRequest(new { Message = "invalid student id." });
            }

            string sql = "SELECT user_password, national_id, system_user_id FROM student INNER JOIN system_user" + "\n"
                    + "WHERE student.student_id = system_user.system_user_id" + "\n"
                    + "AND system_user.system_user_id = @ID;";
            int success;
            List<Student> students;

            students = _data.LoadData<Student, dynamic>(sql, new { ID = studentID },
                _config.GetConnectionString("Default"));

            if (students == null)
            {
                return BadRequest(new { message = "unknown error, maybe database server is down." });
            }
            else if (students.Count == 0)
            {
                return NotFound(new { message = "student does not exist." });
            }
            else if (students[0].User_password != null)
            {
                return BadRequest(new { message = "student already has an account." });
            }
            else
            {
                sql = "UPDATE system_user SET user_password = @pass WHERE system_user_id = @ID;";
                string pass = SecurityUtilities.HashPassword(students[0].National_id);

                success = _data.SaveData<dynamic>(sql, new { pass, ID = students[0].System_user_id },
                    _config.GetConnectionString("Default"));
                if (success > 0)
                {
                    return Ok(new { Message = "account was created successfully." });
                }
                else
                {
                    return BadRequest(new { message = "could not update." });
                }
            }
        }

        [Authorize(Roles = "admin")]
        [HttpPost("CreateAccountForInstructor")]
        public IActionResult CreateAccountForInstructor([FromBody] dynamic inputData)
        {
            JsonElement inputJson = (JsonElement)inputData;

            if (!CreateAccountForInstructorDataExist(inputJson))
                return BadRequest(new { Message = "sent data is not complete." });

            int instructorID = inputJson.GetProperty("InstructorID").GetInt32();
            if (HelperFunctions.GetUserTypeFromID(instructorID) != "instructor")
            {
                return BadRequest(new { Message = "invalid instructor id." });
            }

            string sql = "SELECT user_password, national_id, system_user_id FROM instructor INNER JOIN system_user" + "\n"
                    + "WHERE instructor.instructor_id = system_user.system_user_id" + "\n"
                    + "AND system_user.system_user_id = @ID;";

            List<Instructor> instructors;
            instructors = _data.LoadData<Instructor, dynamic>(sql, new { ID = instructorID }, _config.GetConnectionString("Default"));

            if (instructors == null)
            {
                return BadRequest(new { Message = "unknown error, maybe database server is down." });
            }
            else if (instructors.Count == 0)
            {
                return NotFound(new { Message = "instructor does not exist." });
            }
            else if (instructors[0].User_password != null)
            {
                return BadRequest(new { Message = "instructor already has an account." });
            }
            else
            {
                sql = "UPDATE system_user SET user_password = @pass where system_user_id = @ID;";
                int success;
                string pass = SecurityUtilities.HashPassword(instructors[0].National_id);

                success = _data.SaveData<dynamic>(sql, new { pass, ID = instructors[0].System_user_id },
                    _config.GetConnectionString("Default"));

                if (success > 0)
                {

                    return Ok(new { Messsage = "account was created successfully." });
                }
                else
                {
                    return BadRequest(new { Message = "could not update." });
                }
            }
        }

        /// <summary>
        /// Reset password of account to the national id of the user
        /// </summary>
        /// <param name="userData">json object containing id national_id and type.</param>
        /// <returns>Ok if successful BadRequest otherwise.</returns>
        [Authorize(Roles = "admin")]
        [HttpPatch("ResetAccountCredientials")]
        public IActionResult ResetAccountCredientials([FromBody] JsonElement userData)
        {
            if(!ResetAccountCredientialsRequiredDataValid(userData))
            {
                return BadRequest(new { Message = "required data missing or invalid." });
            }

            int userID = userData.GetProperty("UserID").GetInt32();
            string nationalID = userData.GetProperty("NationalID").GetString();
            string type = userData.GetProperty("Type").GetString();

            if (!HelperFunctions.CheckUserIDAndType(userID, type))
            {
                return BadRequest(new { Message = "invalid user id or type." });
            }

            string sql;
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
            sql = $"SELECT system_user_id, user_password, national_id FROM {table} INNER JOIN system_user" + "\n"
                    + $"WHERE {id_text} = system_user.system_user_id" + "\n"
                    + "AND system_user.system_user_id = @ID" + "\n"
                    + "AND system_user.national_id = @NationalID;";

            dynamic dbResult = null;

            switch (type)
            {
                case "student":
                    dbResult = _data.LoadData<Student, dynamic>(sql, new { ID = userID, NationalID = nationalID }, _config.GetConnectionString("Default"));
                    break;
                case "instructor":
                    dbResult = _data.LoadData<Instructor, dynamic>(sql, new { ID = userID, NationalID = nationalID }, _config.GetConnectionString("Default"));
                    break;
                case "admin":
                    dbResult = _data.LoadData<Admin_user, dynamic>(sql, new { ID = userID, NationalID = nationalID }, _config.GetConnectionString("Default"));
                    break;
            }

            if (dbResult == null)
            {
                return BadRequest(new { Message = "unknown error, maybe database server is down." });
            }
            else if (dbResult.Count == 0)
            {
                return NotFound(new { Message = "system user does not exist." });
            }
            else
            {
                System_user systemUser = dbResult[0];

                if (systemUser.User_password == null)
                {
                    return BadRequest(new { Message = "user does not have an account yet, create one first." });
                }

                string pass = SecurityUtilities.HashPassword(systemUser.National_id);
                sql = "UPDATE system_user SET user_password = @pass WHERE system_user_id = @ID;";

                int success;
                success = _data.SaveData<dynamic>(sql, new { pass, ID = systemUser.System_user_id },
                    _config.GetConnectionString("Default"));

                if (success >= 0)
                {
                    return Ok(new { Message = "account credientials was reset sucessfully." });
                }
                else
                {
                    return BadRequest(new { Message = "could not update." });
                }
            }
        }

        private bool ResetAccountCredientialsRequiredDataValid(JsonElement userData)
        {
            return userData.TryGetProperty("UserID", out JsonElement temp) && temp.TryGetInt32(out _)
                && userData.TryGetProperty("NationalID", out temp) && temp.ValueKind == JsonValueKind.String
                && userData.TryGetProperty("Type", out temp) && temp.ValueKind == JsonValueKind.String;
        }

        private static bool CreateAccountForStudentDataExist(JsonElement sentData)
        {
            return sentData.TryGetProperty("StudentID", out JsonElement temp) && temp.TryGetInt32(out _);
        }

        private static bool CreateAccountForInstructorDataExist(JsonElement sentData)
        {
            return sentData.TryGetProperty("InstructorID", out JsonElement temp) && temp.TryGetInt32(out _);
        }

        private static bool LoginRequiredDataValid(JsonElement sentData)
        {
            return sentData.TryGetProperty("UserID", out JsonElement temp) && temp.TryGetInt32(out _)
                && sentData.TryGetProperty("Password", out temp) && temp.ValueKind == JsonValueKind.String
                && sentData.TryGetProperty("Type", out temp) && temp.ValueKind == JsonValueKind.String;
        }

        private string GenerateJSONWebToken(int userID, string type)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[] {
                new Claim(ClaimTypes.Role, type),
                new Claim("UserID", "" + userID)
            };
            JwtSecurityToken token = new(_config["Jwt:Issuer"],
              null,
              claims,
              expires: DateTime.Now.AddMinutes(60),
              signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private bool AuthenticateUser(int id, string password, string type)
        {
            var parameters = new
            {
                ID = id,
            };
            type = type.ToLower();

            string table;
            string id_text;
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
                    return false;
            }

            string sql = "SELECT system_user_id, user_password" + "\n"
                    + $"FROM {table} INNER JOIN system_user" + "\n"
                    + $"WHERE {id_text} = system_user.system_user_id" + "\n"
                    + "AND system_user.system_user_id = @ID";
            dynamic rows;

            rows = _data.LoadData<dynamic, dynamic>(sql, parameters, _config.GetConnectionString("Default"));

            if (rows != null && rows.Count > 0)
            {
                return SecurityUtilities.Verify(password, rows[0].user_password);
            }
            else
            {
                return false;
            }
        }
    }
}
