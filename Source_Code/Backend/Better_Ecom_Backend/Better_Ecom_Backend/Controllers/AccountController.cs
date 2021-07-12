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
        private IConfiguration _config;
        private IDataAccess _data;

        public AccountController(IConfiguration config, IDataAccess data)
        {
            _config = config;
            _data = data;
        }

        [HttpPost("Login")]
        public IActionResult Login([FromBody] dynamic loginData)
        {
            JsonElement loginDataJSON = (JsonElement)loginData;
            int id = loginDataJSON.GetProperty("ID").GetInt32();
            string password = loginDataJSON.GetProperty("Password").GetString();
            string type = loginDataJSON.GetProperty("Type").GetString();

            //IActionResult
            IActionResult response = Unauthorized();
            bool exists = AuthenticateUser(id, password, type) && password != "";

            if (exists)
            {
                string tokenString = GenerateJSONWebToken(id, type);
                response = Ok(new { token = tokenString });
            }
            return response;
        }
        [Authorize]
        [HttpPost("CreateAccountForStudent")]
        public IActionResult CreateAccountForStudent([FromBody] dynamic inputData)
        {
            JsonElement inputJson = (JsonElement)inputData;
            int studentID = inputJson.GetProperty("StudentID").GetInt32();
            string sql = "SELECT * FROM student INNER JOIN system_user" + "\n"
                    + "WHERE student.student_id = system_user.system_user_id" + "\n"
                    + "AND system_user.system_user_id = @ID;";
            int success;
            Student student;

            student = _data.LoadData<Student, dynamic>(sql, new { ID = studentID },
                _config.GetConnectionString(Constants.CurrentDBConnectionStringName)).FirstOrDefault();
            if (student != null && student.User_password == null)
            {
                sql = "UPDATE system_user SET user_password = @pass WHERE system_user_id = @ID;";
                string pass = student.National_id;

                success = _data.SaveData<dynamic>(sql, new { pass = pass, ID = student.System_user_id },
                    _config.GetConnectionString(Constants.CurrentDBConnectionStringName));
                if (success > 0)
                {
                    student.User_password = pass;
                    return Ok(student);
                }
                else
                {
                    return BadRequest(new { message = "couldn't update." });
                }
            }
            else
            {
                if (student == null)
                {
                    return NotFound(new { message = "student doesn't exist." });
                }
                else if (student.User_password != null)
                {
                    return BadRequest(new { message = "student already has an account." });
                }
                else
                {
                    return BadRequest(new { message = "unknown error." });
                }
            }
        }
        [Authorize]
        [HttpPost("CreateAccountForInstructor")]
        public IActionResult CreateAccountForInstructor([FromBody] dynamic inputData)
        {
            JsonElement inputJson = (JsonElement)inputData;
            int instructorID = inputJson.GetProperty("InstructorID").GetInt32();
            Instructor instructor;

            string sql = "SELECT * FROM instructor INNER JOIN system_user" + "\n"
                    + "WHERE instructor.instructor_id = system_user.system_user_id" + "\n"
                    + "AND system_user.system_user_id = @ID;";

            instructor = _data.LoadData<Instructor, dynamic>(sql, new { ID = instructorID }, _config.GetConnectionString(Constants.CurrentDBConnectionStringName)).FirstOrDefault();

            if (instructor != null && instructor.User_password == null)
            {
                sql = "UPDATE system_user SET user_password = @pass where system_user_id = @ID;";
                int success;
                string pass = instructor.National_id;

                success = _data.SaveData<dynamic>(sql, new { pass = pass, ID = instructor.System_user_id },
                    _config.GetConnectionString(Constants.CurrentDBConnectionStringName));

                if (success > 0)
                {
                    instructor.User_password = pass;
                    return Ok(instructor);
                }
                else
                {
                    return BadRequest(new { message = "couldn't update." });
                }
            }
            else
            {
                if (instructor == null)
                {
                    return NotFound(new { message = "instructor doesn't exist." });
                }
                else if (instructor.User_password != null)
                {
                    return BadRequest(new { message = "instructor already has an account." });
                }
                return BadRequest(new { message = "unknown error." });
            }
        }
        [Authorize]
        [HttpPatch("ResetAccountCredientials")]
        public IActionResult ResetAccountCredientials([FromBody] dynamic userData)
        {
            JsonElement userJson = (JsonElement)userData;
            int id = userJson.GetProperty("ID").GetInt32();
            string nationalID = userJson.GetProperty("NationalID").GetString();
            string type = userJson.GetProperty("Type").GetString();
            System_user systemUser = null;

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
                default:
                    return BadRequest(new { Message = "invalid user type." });
            }
            sql = $"SELECT * FROM {table} INNER JOIN system_user" + "\n"
                    + $"WHERE {id_text} = system_user.system_user_id" + "\n"
                    + "AND system_user.system_user_id = @ID" + "\n"
                    + "AND system_user.national_id = @NationalID;";

            switch (type)
            {
                case "student":
                    systemUser = _data.LoadData<Student, dynamic>(sql, new { ID = id, NationalID = nationalID }, _config.GetConnectionString(Constants.CurrentDBConnectionStringName)).FirstOrDefault();
                    break;
                case "instructor":
                    systemUser = _data.LoadData<Instructor, dynamic>(sql, new { ID = id, NationalID = nationalID }, _config.GetConnectionString(Constants.CurrentDBConnectionStringName)).FirstOrDefault();
                    break;
            }
            if (systemUser != null)
            {
                sql = "UPDATE system_user SET user_password = @pass WHERE system_user_id = @ID;";
                int success;
                string pass = systemUser.National_id;

                success = _data.SaveData<dynamic>(sql, new { pass = pass, ID = systemUser.System_user_id },
                    _config.GetConnectionString(Constants.CurrentDBConnectionStringName));

                if (success >= 0)
                {
                    systemUser.User_password = pass;
                    return Ok(systemUser);
                }
                else
                {

                    

                    return BadRequest(new { message = "couldn't update." });
                }
            }
            else
            {
                if (systemUser == null)
                {
                    return NotFound(new { message = "system user doesn't exist." });
                }
                return BadRequest(new { message = "unknown error." });
            }
        }

        private string GenerateJSONWebToken(int id, string type)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[] {
                new Claim(ClaimTypes.Role, type),
                new Claim("ID", "" + id)
            };
            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
              null,
              claims,
              expires: DateTime.Now.AddMinutes(120),
              signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private bool AuthenticateUser(int id, string password, string type)
        {
            var parameters = new
            {
                ID = id,
                password = password,
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

            string sql = "SELECT system_user_id" + "\n"
                    + $"FROM {table} INNER JOIN system_user" + "\n"
                    + $"WHERE {id_text} = system_user.system_user_id" + "\n"
                    + "AND system_user.system_user_id = @ID" + "\n"
                    + "AND system_user.user_password = @password";
            List<int> rows;

            rows = _data.LoadData<int, dynamic>(sql, parameters, _config.GetConnectionString(Constants.CurrentDBConnectionStringName));
            if (rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
