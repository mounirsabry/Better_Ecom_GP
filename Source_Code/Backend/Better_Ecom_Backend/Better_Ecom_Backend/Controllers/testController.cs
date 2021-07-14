using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Better_Ecom_Backend.Models;
using Microsoft.Extensions.Configuration;
using DataLibrary;
using Better_Ecom_Backend.Helpers;

namespace Better_Ecom_Backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class testController : Controller
    {
        IConfiguration _config;
        IDataAccess _data;

        public testController(IConfiguration config, IDataAccess data)
        {
            _config = config;
            _data = data;
        }

        public IActionResult test()
        {
            string sql = "SELECT * FROM admin_user;";

            var admins = _data.LoadData<Admin_user, dynamic>(sql, new { }, _config.GetConnectionString("Default"));
            return Ok(admins.ToList());
        }
    }
}
