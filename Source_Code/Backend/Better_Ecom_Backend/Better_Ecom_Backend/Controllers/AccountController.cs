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

        [HttpPost("login")]
        public IActionResult Login([FromBody] dynamic loginData)
        {
            JsonElement loginDataJSON = (JsonElement)loginData;
            int id = loginDataJSON.GetProperty("ID").GetInt32();
            string password = loginDataJSON.GetProperty("Password").GetString();
            string type = loginDataJSON.GetProperty("Type").GetString();

            //IActionResult
            IActionResult response = Unauthorized();
            bool exists = AuthenticateUser(id, password, type);

            if (exists)
            {
                string tokenString = GenerateJSONWebToken(id, password, type);
                response = Ok(new { token = tokenString });
            }
            return response;
        }

        private string GenerateJSONWebToken(int id, string password, string type)
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

            string sql = @$"SELECT system_user_id 
                    FROM {table} INNER JOIN system_user
                    WHERE {id_text} = system_user.system_user_id 
                    AND system_user.system_user_id = @ID 
                    AND system_user.user_password = @password";
            List<int> rows = _data.LoadData<int, dynamic>(sql, parameters, _config.GetConnectionString(Constants.CurrentDBConnectionStringName));

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
