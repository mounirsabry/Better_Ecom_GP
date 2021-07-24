using Better_Ecom_Backend.Entities;
using Better_Ecom_Backend.Helpers;
using Better_Ecom_Backend.Models;
using DataLibrary;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
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
            string sql = "SELECT * FROM admin_user WHERE admin_user_id = 11;";

            var admins = _data.LoadData<Admin_user, dynamic>(sql, new { }, _config.GetConnectionString("Default"));
            return Ok(admins.ToList());
        }

        [HttpGet("TestExistenceFunction/{ID}")]
        public IActionResult TestExistenceFunction(int id)
        {
            return Ok(ExistanceFunctions.IsCourseInstanceExists(_config, _data, id));
        }

        [HttpGet("TestDBUpAndRunning")]
        public IActionResult TestDBUpAndRunning()
        {
            return Ok(ExistanceFunctions.IsDBUpAndRunning(_config, _data));
        }

        [HttpGet("TestSomeFunction")]
        public IActionResult TestSomeFunction()
        {
            Term someTerm = Term.First;
            Console.WriteLine(someTerm);
            Console.WriteLine(Term.First);
            Console.WriteLine(someTerm.GetType());
            Console.WriteLine(Term.First.GetType());
            Console.WriteLine("");
            Console.WriteLine(nameof(someTerm));
            Console.WriteLine(someTerm + "");
            Console.WriteLine(nameof(someTerm).GetType());
            Console.WriteLine((someTerm + "").GetType());
            Console.WriteLine("");
            Console.WriteLine(Enum.GetName<Term>(someTerm));
            Console.WriteLine(Enum.GetName<Term>(someTerm).GetType());

            return Ok(new { Message = "test finished."} );
        }
    }
}
