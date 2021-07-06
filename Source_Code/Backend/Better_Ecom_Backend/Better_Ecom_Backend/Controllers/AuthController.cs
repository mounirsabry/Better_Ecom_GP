using Better_Ecom_Backend.Helpers;
using Better_Ecom_Backend.Models;
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

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Better_Ecom_Backend.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private IConfiguration _config;

        public AuthController(IConfiguration config)
        {
            _config = config;
        }


        [HttpPost("login")]
        public IActionResult Login([FromBody] LogUser logUser)
        {

            //IActionResult
            IActionResult response = Unauthorized();

            bool exists = AuthenticateUser(logUser);

            if (exists)
            {
                string tokenString = GenerateJSONWebToken(logUser);
                response = Ok(new { token = tokenString});
            }

            return response;
        }

        private string GenerateJSONWebToken(LogUser logUser)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[] {
                new Claim(ClaimTypes.Role, logUser.Type),
            };
            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
              null,
              claims,
              expires: DateTime.Now.AddMinutes(120),
              signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private bool AuthenticateUser(LogUser logUser)
        {
            string table = "";
            string sql = "";
            var parameters = new
            {
                ID = logUser.ID,
                password = logUser.Password,

            };
            DataAcess dataAcess = new DataAcess(_config.GetConnectionString(Constants.CurrentDatabaseConnectionString));

            string type = logUser.Type.ToLower();
            string id_text = "";
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
            
            sql = @$"SELECT system_user_id FROM {table} INNER JOIN system_user
                    WHERE {id_text} = system_user.system_user_id 
                    AND system_user.system_user_id = @ID AND system_user.user_password = @password";

            List<int> rows = dataAcess.selectData<int, dynamic>(sql, parameters);

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
