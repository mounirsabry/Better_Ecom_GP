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
    public class AttendanceController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IDataAccess _data;

        public AttendanceController(IConfiguration config, IDataAccess data)
        {
            _config = config;
            _data = data;
        }

        [Authorize]
        [HttpGet("GetStudentCourseInstanceAttendance/{CourseInstanceID:int}/{StudentID:int}")]
        public IActionResult GetStudentCourseInstanceAttendance([FromHeader] string Authorization, int courseInstanceID, int studentID)
        {
            TokenInfo token = HelperFunctions.GetIdAndTypeFromToken(Authorization);
            if (token.Type == "instructor")
            {
                if (RegistrationFunctions.IsInstructorRegisteredToCourseInstance(_config, _data, token.UserID, courseInstanceID))
                {
                    return Forbid("instructor not registered in course instance.");
                }
            }
            if (token.Type == "student")
            {
                if (token.UserID != studentID)
                {
                    return Forbid("students can only view their data.");
                }
            }


            string getStudentAttendanceSql = $"SELECT * FROM student_attendance_item_attendance INNER JOIN attendance_item ON attendance_item_id = item_id" + "\n" +
                $" WHERE student_id = @studentID AND course_instance_id = @courseInstanceID;";

            List<StudentAttendanceItemAndInfo> attendances = _data.LoadData<StudentAttendanceItemAndInfo, dynamic>(getStudentAttendanceSql, new { courseInstanceID, studentID },
                _config.GetConnectionString("Default"));

            if (attendances is null)
            {
                return BadRequest(new { Message = MessageFunctions.GetMaybeDatabaseIsDownMessage() });
            }
            else
            {
                return Ok(attendances);
            }
        }

        [Authorize(Roles = "instructor, admin")]
        [HttpPost("AddCourseInstanceAttendanceItem")]
        public IActionResult AddCourseInstanceAttendanceItem([FromHeader] string Authorization, [FromBody] JsonElement jsonInput)
        {
            if(CheckAddCourseInstanceAttendanceItemDataValid(jsonInput) == false)
            {
                return BadRequest(new { Message = MessageFunctions.GetRequiredDataMissingOrInvalidMessage() });
            }

            int courseInstanceID = jsonInput.GetProperty("CourseInstanceID").GetInt32();
            AttendanceType attendanceType = (AttendanceType)jsonInput.GetProperty("AttendanceType").GetInt32();

            TokenInfo token = HelperFunctions.GetIdAndTypeFromToken(Authorization);
            if (token.Type == "instructor")
            {
                if (RegistrationFunctions.IsInstructorRegisteredToCourseInstance(_config, _data, token.UserID, courseInstanceID))
                {
                    return Forbid("instructor not registered in course instance.");
                }
            }

            Attendance_item item = new();

            item.Course_instance_id = courseInstanceID;
            item.Attendance_type = attendanceType;
            item.Item_name = jsonInput.GetProperty("ItemName").GetString();

            if(jsonInput.TryGetProperty("AttendanceDate", out JsonElement temp))
            {
                if(!temp.TryGetDateTime(out _))
                {
                    return BadRequest(new { Message = "invalid date." });
                }

                item.Attendance_date = temp.GetDateTime();
            }
            else
            {
                item.Attendance_date = DateTime.Now;
            }


            string insertAttendanceItem = "INSERT INTO attendance_item VALUES(NULL, @Course_instance_id, @Item_name, @Attendance_type, @Attendance_date);";
            var parameters = new
            {
                item.Course_instance_id,
                item.Item_name,
                item.Attendance_date,
                Attendance_type = Enum.GetName(attendanceType)
            };

            int status1 = _data.SaveData(insertAttendanceItem, parameters, _config.GetConnectionString("Default"));

            if(status1 <= 0 )
            {
                return BadRequest(new { Message = MessageFunctions.GetMaybeDatabaseIsDownMessage() });
            }


            List<int> itemIds = GetAttendanceItemID(courseInstanceID, item.Item_name);

            if( itemIds is null || itemIds.Count == 0)
            {
                return BadRequest(new { Message = MessageFunctions.GetMaybeDatabaseIsDownMessage() });
            }

            int attendanceItemID = itemIds.First();

            List<string> sqlList = new();
            List<dynamic> parametersList = new();

            List<int> ids = GetStudentIdsInCourseInstance(courseInstanceID);
            
            if(ids is null)
            {
                return BadRequest(new { Message = MessageFunctions.GetMaybeDatabaseIsDownMessage() });
            }

            foreach(int id in ids)
            {
                sqlList.Add("INSERT INTO student_attendance_item_attendance VALUES(@attendanceItemID, @id, @attendanceStatus);");
                parametersList.Add(new { attendanceItemID, id, attendanceStatus = Enum.GetName(AttendanceStatus.Not_Specified) });
            }

            List<int> status = _data.SaveDataTransaction(sqlList, parametersList, _config.GetConnectionString("Default"));

            if(status.Contains(-1))
            {
                return BadRequest(new { Message = MessageFunctions.GetMaybeDatabaseIsDownMessage() });
            }
            else
            {
                return Ok();
            }

        }


        [Authorize(Roles = "instructor, admin")]
        [HttpPatch("SetAttendanceItemAttendanceForStudent")]
        public IActionResult SetAttendanceItemAttendanceForStudent([FromHeader] string Authorization, [FromBody] JsonElement jsonInput)
        {
            if (CheckSetAttendanceItemAttendanceForStudentDataValid(jsonInput) == false)
            {
                return BadRequest(new { Message = MessageFunctions.GetRequiredDataMissingOrInvalidMessage() });
            }

            int courseInstanceID = jsonInput.GetProperty("CourseInstanceID").GetInt32();
            AttendanceStatus attendanceStatus = (AttendanceStatus)jsonInput.GetProperty("AttendanceStatus").GetInt32();

            TokenInfo token = HelperFunctions.GetIdAndTypeFromToken(Authorization);
            if (token.Type == "instructor")
            {
                if (RegistrationFunctions.IsInstructorRegisteredToCourseInstance(_config, _data, token.UserID, courseInstanceID))
                {
                    return Forbid("instructor not registered in course instance.");
                }
            }


            int attendanceItemID = jsonInput.GetProperty("AttendanceItemID").GetInt32();
            int studentID = jsonInput.GetProperty("StudentID").GetInt32();

            string setAttendanceStatusSql = "UPDATE student_attendance_item_attendance SET attendance_status = @attendanceStatus WHERE attendance_item_id = @itemID AND student_id = @studentID;";
            int status = _data.SaveData(setAttendanceStatusSql, 
                new { itemID = attendanceItemID, studentID, attendanceStatus = Enum.GetName(attendanceStatus) },
                _config.GetConnectionString("Default"));

            if(status >= 0)
            {
                return Ok();
            }
            else
            {
                return BadRequest(new { Message = MessageFunctions.GetMaybeDatabaseIsDownMessage() });
            }

        }


        [Authorize(Roles = "instructor, admin")]
        [HttpDelete("DeleteCourseInstanceAttendanceItem/{CourseInstanceID:int}/{ItemID:int}")]
        public IActionResult DeleteCourseInstanceAttendanceItem([FromHeader] string Authorization, int courseInstanceID, int itemID)
        {

            if(ExistanceFunctions.IsCourseInstanceExists(_config,_data,courseInstanceID) == false)
            {
                return BadRequest(new { Message = MessageFunctions.GetCourseInstanceNotFoundMessage() });
            }

            TokenInfo tokenInfo = HelperFunctions.GetIdAndTypeFromToken(Authorization);
            if (tokenInfo.Type == "instructor")
            {
                if (RegistrationFunctions.IsInstructorRegisteredToCourseInstance(_config, _data, tokenInfo.UserID, courseInstanceID) == false)
                {
                    return BadRequest(new { Message = MessageFunctions.GetInstructorNotRegisteredToCourseInstanceMessage() });
                }
            }

            string deleteAttendanceItemSql = "DELETE FROM attendance_item WHERE item_id = @itemID; ";

            int status = _data.SaveData(deleteAttendanceItemSql, new { itemID }, _config.GetConnectionString("Default"));

            if(status >= 0 )
            {
                return Ok();
            }
            else
            {
                return BadRequest(new { Message = MessageFunctions.GetMaybeDatabaseIsDownMessage() });
            }

        }




        private List<int> GetAttendanceItemID(int courseInstanceID, string itemName)
        {
            return _data.LoadData<int, dynamic>("SELECT item_id FROM attendance_item WHERE course_instance_id = @courseInstanceID AND item_name = @itemName;",
                new { courseInstanceID, itemName }, _config.GetConnectionString("Default"));
        }
        private static bool CheckSetAttendanceItemAttendanceForStudentDataValid(JsonElement jsonInput)
        {
            return jsonInput.TryGetProperty("AttendanceItemID", out JsonElement temp) && temp.TryGetInt32(out _)
                && jsonInput.TryGetProperty("StudentID", out temp) && temp.TryGetInt32(out _)
                && jsonInput.TryGetProperty("AttendanceStatus",out temp) && temp.TryGetInt32(out int type) && Enum.IsDefined((AttendanceStatus)type);
        }

        List<int> GetStudentIdsInCourseInstance(int courseInstanceID)
        {
            return _data.LoadData<int, dynamic>("SELECT student_id FROM student_course_instance_registration WHERE course_instance_id = @courseInstanceID",
                new { courseInstanceID }, _config.GetConnectionString("Default"));
        }

        private static bool CheckAddCourseInstanceAttendanceItemDataValid(JsonElement jsonInput)
        {
            return jsonInput.TryGetProperty("CourseInstanceID", out JsonElement temp) && temp.TryGetInt32(out _)
                && jsonInput.TryGetProperty("ItemName", out temp) && temp.ValueKind == JsonValueKind.String
                && jsonInput.TryGetProperty("AttendanceType", out temp) && temp.TryGetInt32(out int type) && Enum.IsDefined((AttendanceType)type);
        }
    }
}
