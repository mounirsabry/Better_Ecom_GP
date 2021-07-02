using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Better_Ecom_Backend.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {

        private IConfiguration _config;

        public ProfileController(IConfiguration config)
        {
            _config = config;
        }

        [Authorize]
        [HttpGet("{type}/{ID:int}")]
        public dynamic getData(string type, int id)
        {
            DataAcess dataAcess = new DataAcess(_config.GetConnectionString("DB"));

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


           return dataAcess.selectData<dynamic, dynamic>(sql, new { ID = id }).FirstOrDefault();


        }
    }
}
