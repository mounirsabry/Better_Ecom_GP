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
        
        
        
        public IActionResult test()
        {
            return Ok(new { s = "string" });
        }

        
    }
}
