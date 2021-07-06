using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Better_Ecom_Backend.Models;
using Microsoft.Extensions.Configuration;

namespace Better_Ecom_Backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class testController : Controller
    {
        IConfiguration _configuration;

        public testController(IConfiguration configuration)
        {
            this._configuration = configuration;

        }
        
        [HttpGet]
        public List<System_user> get()
        {
            string sql = @"SELECT * FROM system_user";
            return DataAcess.LoadData<System_user, dynamic>(sql, new {}, _configuration.GetConnectionString("Remote_MySQL_COM_DB"));
        }
    }
}
