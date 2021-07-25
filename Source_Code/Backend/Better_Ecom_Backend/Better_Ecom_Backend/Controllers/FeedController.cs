using Better_Ecom_Backend.Entities;
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
using System.Text.Json;
using System.Threading.Tasks;

namespace Better_Ecom_Backend.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class FeedController : ControllerBase
    {

        private readonly IConfiguration _config;
        private readonly IDataAccess _data;

        public FeedController(IConfiguration config, IDataAccess data)
        {
            _config = config;
            _data = data;
        }

      
        [HttpGet("GetGeneralFeeds")]
        public IActionResult GetGeneralFeeds()
        {
            string getGeneralFeedSql = "Select * FROM general_feed;";

            if(ExistanceFunctions.IsDBUpAndRunning(_config,_data) == false)
            {
                return BadRequest(new { Message = MessageFunctions.GetMaybeDatabaseIsDownMessage() });
            }

            List<General_feed> feed = _data.LoadData<General_feed, dynamic>(getGeneralFeedSql, new { }, _config.GetConnectionString("Default"));

            if (feed is null)
            {
                return BadRequest(new { Message = MessageFunctions.GetMaybeDatabaseIsDownMessage() });
            }

            return Ok(feed);
        }

        [Authorize(Roles = "admin")]
        [HttpPost("AddToGeneralFeeds")]
        public IActionResult AddToGeneralFeeds([FromBody] JsonElement jsonInput)
        {
            if (ExistanceFunctions.IsDBUpAndRunning(_config, _data) == false)
            {
                return BadRequest(new { Message = MessageFunctions.GetMaybeDatabaseIsDownMessage() });
            }

            bool wasContentSent = jsonInput.TryGetProperty(nameof(General_feed.Content), out JsonElement temp) && temp.ValueKind == JsonValueKind.String;
            if (wasContentSent == false)
            {
                return BadRequest("Content string is missing.");
            }
            string content = jsonInput.GetProperty(nameof(General_feed.Content)).GetString();
            DateTime insertionDate = DateTime.Now;
            string insertIntoGeneralFeed = "INSERT INTO general_feed VALUES(NULL, @content, @insertionDate);";

            int status = _data.SaveData(insertIntoGeneralFeed, new { content, insertionDate }, _config.GetConnectionString("Default"));

            if (status > 0)
            {
                return Ok();
            }
            else
            {
                return BadRequest(new { Message = MessageFunctions.GetMaybeDatabaseIsDownMessage() });
            }
        }

        [Authorize(Roles ="admin")]
        [HttpDelete("DeleteFromGeneralFeeds/{FeedID:int}")]
        public IActionResult DeleteFromGeneralFeeds(int feedID)
        {
            if (ExistanceFunctions.IsDBUpAndRunning(_config, _data) == false)
            {
                return BadRequest(new { Message = MessageFunctions.GetMaybeDatabaseIsDownMessage() });
            }
            string deleteFromGeneralFeedSql = "DELETE FROM general_feed WHERE feed_id = @feedID;";
            int status = _data.SaveData(deleteFromGeneralFeedSql, new { feedID }, _config.GetConnectionString("Default"));
            if (status >= 0)
            {
                return Ok();
            }
            else
            {
                return BadRequest(new { Message = MessageFunctions.GetMaybeDatabaseIsDownMessage() });
            }
        }

        [Authorize]
        [HttpGet("GetCourseInstanceFeed/{CourseInstanceID:int}")]
        public IActionResult GetCourseInstanceFeed([FromHeader] string Authorization, int courseInstanceID)
        {

            if (ExistanceFunctions.IsDBUpAndRunning(_config, _data) == false)
            {
                return BadRequest(new { Message = MessageFunctions.GetMaybeDatabaseIsDownMessage() });
            }

            TokenInfo tokenInfo = HelperFunctions.GetIdAndTypeFromToken(Authorization);
            if (tokenInfo.Type == "student")
            {
                if (RegistrationFunctions.IsStudentRegisteredToCourseInstance(_config,_data,tokenInfo.UserID, courseInstanceID) == false)
                {
                    return Forbid("student must be registered in course.");
                }
            }

            if (tokenInfo.Type == "instructor")
            {
                if (RegistrationFunctions.IsInstructorRegisteredToCourseInstance(_config,_data,tokenInfo.UserID, courseInstanceID) == false)
                {
                    return Forbid("instructor must be registered in course.");
                }
            }

            if (ExistanceFunctions.IsCourseInstanceExists(_config, _data, courseInstanceID) == false)
            {
                return BadRequest(new { Message = MessageFunctions.GetCourseInstanceNotFoundMessage() });
            }

            string getCourseInstanceFeed = "SELECT * FROM course_instance_feed WHERE course_instance_id = @courseInstanceID;";
            List<Course_instance_feed> courseFeeds = _data.LoadData<Course_instance_feed, dynamic>(getCourseInstanceFeed, new { courseInstanceID }, _config.GetConnectionString("Default"));

            if (courseFeeds is null)
            {
                return BadRequest(new { Message = MessageFunctions.GetMaybeDatabaseIsDownMessage() });
            }

            return Ok(courseFeeds);
        }

        [Authorize(Roles = "instructor, admin")]
        [HttpPost("AddToCourseInstanceFeed")]
        public IActionResult AddToCourseInstanceFeed([FromHeader] string Authorization, JsonElement jsonInput)
        {
            if (ExistanceFunctions.IsDBUpAndRunning(_config, _data) == false)
            {
                return BadRequest(new { Message = MessageFunctions.GetMaybeDatabaseIsDownMessage() });
            }

            if(CheckAddToCourseInstanceFeedDataValid(jsonInput) == false)
            {
                return BadRequest(new { Message = MessageFunctions.GetRequiredDataMissingOrInvalidMessage() });
            }

            int courseInstanceID = jsonInput.GetProperty("CourseInstanceID").GetInt32();
            string content = jsonInput.GetProperty("Content").GetString();
            DateTime insertionTime = DateTime.Now;

            TokenInfo tokenInfo = HelperFunctions.GetIdAndTypeFromToken(Authorization);
            if (tokenInfo.Type == "instructor")
            {
                if (RegistrationFunctions.IsInstructorRegisteredToCourseInstance(_config,_data,tokenInfo.UserID, courseInstanceID) == false)
                {
                    return Forbid("instructor must be registered in course.");
                }
            }


            if(ExistanceFunctions.IsCourseInstanceExists(_config,_data,courseInstanceID) == false)
            {
                return BadRequest(new { Message = MessageFunctions.GetCourseInstanceNotFoundMessage() });
            }

            string insertIntoCourseInstanceFeedSql = "INSERT INTO course_instance_feed VALUES(NULL, @courseInstanceID, @content, @insertionTime)";

            int status = _data.SaveData(insertIntoCourseInstanceFeedSql, new { courseInstanceID, content, insertionTime }, _config.GetConnectionString("Default"));

            if(status > 0 )
            {
                return Ok();
            }
            else
            {
                return BadRequest(new { Message = MessageFunctions.GetMaybeDatabaseIsDownMessage() });
            }



        }

        [Authorize(Roles ="admin, instructor")]
        [HttpDelete("DeleteFromCourseInstanceFeed/{courseInstanceID:int}/{FeedID:int}")]
        public IActionResult DeleteFromCourseInstanceFeed([FromHeader] string Authorization, int courseInstanceID, int feedID)
        {
            if (ExistanceFunctions.IsDBUpAndRunning(_config, _data) == false)
            {
                return BadRequest(new { Message = MessageFunctions.GetMaybeDatabaseIsDownMessage() });
            }

            TokenInfo tokenInfo = HelperFunctions.GetIdAndTypeFromToken(Authorization);
            if (tokenInfo.Type == "instructor")
            {
                if (RegistrationFunctions.IsInstructorRegisteredToCourseInstance(_config, _data, tokenInfo.UserID, courseInstanceID) == false)
                {
                    return Forbid("instructor must be registered in course.");
                }
            }
            if (ExistanceFunctions.IsCourseInstanceExists(_config, _data, courseInstanceID) == false)
            {
                return BadRequest(new { Message = MessageFunctions.GetCourseInstanceNotFoundMessage() });
            }

            string deleteCourseInstanceFeedSql = "DELETE FROM course_instance_feed WHERE course_instance_id = @courseInstanceID AND feed_id = @feedID;";

            int status = _data.SaveData(deleteCourseInstanceFeedSql, new { feedID, courseInstanceID }, _config.GetConnectionString("Default"));

            if(status >= 0)
            {
                return Ok();
            }
            else
            {
                return BadRequest(new { Message = MessageFunctions.GetMaybeDatabaseIsDownMessage() });
            }


        }

        private static bool CheckAddToCourseInstanceFeedDataValid(JsonElement jsonInput)
        {
            return jsonInput.TryGetProperty("CourseInstanceID", out JsonElement temp) &&  temp.TryGetInt32(out _)
                && jsonInput.TryGetProperty("Content", out temp) && temp.ValueKind == JsonValueKind.String;

        }


    }
}
