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
        [HttpGet("{type}/{ID:int}")]
        public dynamic getData(string type, int id)
        {
            DataAcess dataAcess = new DataAcess(_config.GetConnectionString(Constants.CurrentDBConnectionStringName));

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

        // not sure if the admin will use this to modify student/instructor profiles, will assume not till i get there.
        [Authorize]
        [HttpPatch("{type}")]
        public IActionResult updateData(string type, [FromBody] dynamic data)
        {
            DataAcess dataAcess = new DataAcess(_config.GetConnectionString(Constants.CurrentDBConnectionStringName));

            bool success = true;


            if (type != "admin" || type != "student" || type != "instructor" ) {

                return BadRequest();

            } else {
                System_user system_user = (System_user)data;
                success = dataAcess.update<System_user>(system_user);

                if (type == "student")
                {
                    Student s = (Student)data;
                    success = dataAcess.update<Student>(s);
                } else if (type == "instructor")
                {
                    Instructor instructor = (Instructor)data;
                    success = dataAcess.update<Instructor>(instructor);
                }
            }


            if (success)
                return Ok();
            else
                return BadRequest();

        }
    }
}
