using Better_Ecom_Backend.Helpers;
using Better_Ecom_Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        [Authorize]
        [HttpGet("GetGeneralFeeds")]
        public IActionResult GetGeneralFeeds()
        {
            return Ok(new { Message = MessageFunctions.GetNotImplementedString() });
        }

        [Authorize(Roles = "admin")]
        [HttpPost("AddToGeneralFeeds")]
        public IActionResult AddToGeneralFeeds([FromBody] JsonElement jsonInput)
        {
            bool wasContentSent = jsonInput.TryGetProperty(nameof(General_feed.Content), out JsonElement temp) && temp.ValueKind == JsonValueKind.String;
            if (wasContentSent == false)
            {
                return BadRequest("Content string is missing.");
            }
            string Content = jsonInput.GetProperty(nameof(General_feed.Content)).GetString();

            return Ok(new { Message = MessageFunctions.GetNotImplementedString() });
        }

        //[Authorize]
    }
}
