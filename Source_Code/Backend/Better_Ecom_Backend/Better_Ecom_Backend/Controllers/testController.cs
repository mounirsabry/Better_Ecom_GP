using Better_Ecom_Backend.Models;
using DataLibrary;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Linq;

namespace Better_Ecom_Backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : Controller
    {
        private readonly IConfiguration _config;
        private readonly IDataAccess _data;

        public TestController(IConfiguration config, IDataAccess data)
        {
            _config = config;
            _data = data;
        }

        public IActionResult Test()
        {
            string sql = "SELECT * FROM admin_user;";

            var admins = _data.LoadData<Admin_user, dynamic>(sql, new { }, _config.GetConnectionString("Default"));
            return Ok(admins.ToList());
        }
    }
}
