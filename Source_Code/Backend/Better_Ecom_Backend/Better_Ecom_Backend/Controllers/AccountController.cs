﻿using Better_Ecom_Backend.Helpers;
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
        public IActionResult Login([FromBody] dynamic loginData)
        {
            JsonElement loginDataJSON = (JsonElement)loginData;
            int id = loginDataJSON.GetProperty("ID").GetInt32();
            string password = loginDataJSON.GetProperty("Password").GetString();
            string type = loginDataJSON.GetProperty("Type").GetString();

            //IActionResult
            IActionResult response = Unauthorized();
            bool exists = AuthenticateUser(id, password, type) && !string.IsNullOrEmpty(password);

            if (exists)
            {
                string tokenString = GenerateJSONWebToken(id, type);
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

            if (CreateAccountForStudentDataExist(inputJson))
            {
                int studentID = inputJson.GetProperty("StudentID").GetInt32();
                string sql = "SELECT user_password, national_id, system_user_id FROM student INNER JOIN system_user" + "\n"
                        + "WHERE student.student_id = system_user.system_user_id" + "\n"
                        + "AND system_user.system_user_id = @ID;";
                int success;
                List<Student> student;

                student = _data.LoadData<Student, dynamic>(sql, new { ID = studentID },
                    _config.GetConnectionString("Default"));

                if (student != null && student.Count > 0 && student[0].User_password == null)
                {
                    sql = "UPDATE system_user SET user_password = @pass WHERE system_user_id = @ID;";
                    string pass = SecurityUtilities.HashPassword(student[0].National_id);

                    success = _data.SaveData<dynamic>(sql, new { pass, ID = student[0].System_user_id },
                        _config.GetConnectionString("Default"));
                    if (success > 0)
                    {
                        return Ok();
                    }
                    else
                    {
                        return BadRequest(new { message = "couldn't update." });
                    }
                }
                else
                {
                    if (student.Count == 0)
                    {
                        return NotFound(new { message = "student doesn't exist." });
                    }
                    else if (student[0].User_password != null)
                    {
                        return BadRequest(new { message = "student already has an account." });
                    }
                    else
                    {
                        return BadRequest(new { message = "unknown error, maybe database server is down." });
                    }
                }
            }

            return BadRequest(new { Message = "sent data is not complete." });
        }

        [Authorize(Roles = "admin")]
        [HttpPost("CreateAccountForInstructor")]
        public IActionResult CreateAccountForInstructor([FromBody] dynamic inputData)
        {
            JsonElement inputJson = (JsonElement)inputData;

            if (!CreateAccountForInstructorDataExist(inputJson))
                return BadRequest(new { Message = "sent data is not complete." });

            int instructorID = inputJson.GetProperty("InstructorID").GetInt32();
            List<Instructor> instructor;

            string sql = "SELECT user_password, national_id, system_user_id FROM instructor INNER JOIN system_user" + "\n"
                    + "WHERE instructor.instructor_id = system_user.system_user_id" + "\n"
                    + "AND system_user.system_user_id = @ID;";



            instructor = _data.LoadData<Instructor, dynamic>(sql, new { ID = instructorID }, _config.GetConnectionString("Default"));

            if (instructor != null && instructor.Count > 0 && instructor[0].User_password == null)
            {
                sql = "UPDATE system_user SET user_password = @pass where system_user_id = @ID;";
                int success;
                string pass = SecurityUtilities.HashPassword(instructor[0].National_id);

                success = _data.SaveData<dynamic>(sql, new { pass, ID = instructor[0].System_user_id },
                    _config.GetConnectionString("Default"));

                if (success > 0)
                {

                    return Ok();
                }
                else
                {
                    return BadRequest(new { Message = "couldn't update." });
                }
            }
            else
            {
                if (instructor.Count == 0)
                {
                    return NotFound(new { Message = "instructor doesn't exist." });
                }
                else if (instructor[0].User_password != null)
                {
                    return BadRequest(new { Message = "instructor already has an account." });
                }
                return BadRequest(new { Message = "unknown error, maybe database server is down." });
            }
        }

        /// <summary>
        /// Reset password of account to the national id of the user
        /// </summary>
        /// <param name="userData">json object containing id national_id and type.</param>
        /// <returns>Ok if successful BadRequest otherwise.</returns>
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
                    dbResult = _data.LoadData<Student, dynamic>(sql, new { ID = id, NationalID = nationalID }, _config.GetConnectionString("Default"));
                    break;
                case "instructor":
                    dbResult = _data.LoadData<Instructor, dynamic>(sql, new { ID = id, NationalID = nationalID }, _config.GetConnectionString("Default"));
                    break;
                case "admin":
                    dbResult = _data.LoadData<Admin_user, dynamic>(sql, new { ID = id, NationalID = nationalID }, _config.GetConnectionString("Default"));
                    break;
            }

            if (dbResult != null && dbResult.Count > 0)
            {
                systemUser = dbResult[0];
            }

            if (systemUser != null)
            {
                sql = "UPDATE system_user SET user_password = @pass WHERE system_user_id = @ID;";
                int success;
                string pass = SecurityUtilities.HashPassword(systemUser.National_id);

                success = _data.SaveData<dynamic>(sql, new { pass, ID = systemUser.System_user_id },
                    _config.GetConnectionString("Default"));

                if (success >= 0)
                {
                    return Ok();
                }
                else
                {
                    return BadRequest(new { Message = "couldn't update." });
                }
            }
            else
            {
                if (systemUser == null)
                {
                    return NotFound(new { Message = "system user doesn't exist." });
                }
                return BadRequest(new { Message = "unknown error, maybe database server is down." });
            }
        }

        private static bool CreateAccountForStudentDataExist(JsonElement sentData)
        {
            return sentData.TryGetProperty("StudentID", out _);
        }

        private static bool CreateAccountForInstructorDataExist(JsonElement sentData)
        {
            return sentData.TryGetProperty("InstructorID", out _);
        }

        private string GenerateJSONWebToken(int id, string type)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[] {
                new Claim(ClaimTypes.Role, type),
                new Claim("ID", "" + id)
            };
            JwtSecurityToken token = new(_config["Jwt:Issuer"],
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
                return SecurityUtilities.Verify(password,rows[0].user_password);
            }
            else
            {
                return false;
            }
        }
    }
}
