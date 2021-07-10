using Better_Ecom_Backend.Models;
using DataLibrary;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Better_Ecom_Backend.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserRegistrationController : ControllerBase
    {
        private IConfiguration _config;
        private IDataAccess _data;

        public UserRegistrationController(IConfiguration config, IDataAccess data)
        {
            _config = config;
            _data = data;
        }

        [HttpPost("AddNewStudent")]
        public IActionResult AddNewStudent([FromBody] dynamic studentData)
        {
            JsonElement studentJson = (JsonElement)studentData;
            Student newStudent = new Student(studentJson);
            Console.WriteLine(newStudent);

            //Adds the new student to the database.
            //The student will have the next available ID in the students table.
            //The four first digits of the ID should be the current Year.
            //The process of adding the student to the students table and the system_users table
            //should be a transaction, which means it should be done all or nothing.

            //Should return the new added student object.
            return Ok("Not Implemented Yet!");
        }

        [HttpPost("AddNewInstructor")]
        public IActionResult AddNewInstructor([FromBody] dynamic instructorData)
        {
            JsonElement instructorJson = (JsonElement)instructorData;
            Instructor newInstructor = new Instructor(instructorJson);

            //Adds the new instructor to the database.
            //The new ID should start with 3 followed by the next available number in
            //the instructors table.
            //Should be a transaction.

            //Should return the new added instructor object.
            return Ok("Not Implemented Yet!");
        }
    }
}
