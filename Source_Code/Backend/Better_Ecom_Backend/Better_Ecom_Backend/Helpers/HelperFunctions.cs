using Better_Ecom_Backend.Entities;
using Better_Ecom_Backend.Models;
using DataLibrary;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

namespace Better_Ecom_Backend.Helpers
{
    public class HelperFunctions
    {
        public static int GetFirstDigit(int number)
        {
            return (int)number.ToString()[0] - 48;
        }

        public static int GetIntNumberOfDigits(int number)
        {
            string numberString = Convert.ToString(number);
            return numberString.Length;
        }

        public static string GetUserTypeFromID(int userID)
        {
            int firstDigit = GetFirstDigit(userID);
            if (firstDigit == 1)
            {
                return "admin";
            }
            else if (firstDigit == 2 && GetIntNumberOfDigits(userID) == 8)
            {
                return "student";
            }
            else if (firstDigit == 3)
            {
                return "instructor";
            }
            else
            {
                return "invalid";
            }
        }

        public static bool CheckUserIDAndType(int userID, string type)
        {
            string deducedType = HelperFunctions.GetUserTypeFromID(userID);
            if (type != deducedType || deducedType == "invalid")
            {
                return false;
            }
            else if (type != "student" && type != "instructor" && type != "admin")
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static TokenInfo GetIdAndTypeFromToken(string tokenString)
        {
            JwtSecurityTokenHandler handler = new();
            tokenString = tokenString[7..];
            JwtSecurityToken token = handler.ReadJwtToken(tokenString);

            return new TokenInfo(int.Parse(token.Claims.ToList()[1].Value), token.Claims.ToList()[0].Value);
        }

        public static bool IsDepartmentCodeValid(IConfiguration _config, IDataAccess _data, string departmentCode)
        {
            if (departmentCode == null)
            {
                return true;
            }
            else if (departmentCode == "")
            {
                return false;
            }

            string sql = "SELECT department_code FROM department WHERE department_code = @DepartmentCode;";
            List<string> departmentCodes = _data.LoadData<string, dynamic>(sql, new { DepartmentCode = departmentCode }, _config.GetConnectionString("Default"));

            if (departmentCodes.Contains(departmentCode))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool CheckDepartmentCodesList(IConfiguration _config, IDataAccess _data, List<string> departmentCodes)
        {
            if (departmentCodes == null || departmentCodes.Count == 0)
            {
                return false;
            }

            string sql = "SELECT department_code FROM department;";
            List<string> availableDepartmentCodes = _data.LoadData<string, dynamic>(sql, new { }, _config.GetConnectionString("Default"));

            foreach (string departmentCode in departmentCodes)
            {
                if (!availableDepartmentCodes.Contains(departmentCode) && departmentCode != null)
                {
                    return false;
                }
            }
            return true;
        }

        public static List<bool> GetCourseInstanceReadOnlyStatusList(IConfiguration _config, IDataAccess _data, int courseInstanceID)
        {
            string sql = "SELECT is_read_only FROM course_instance WHERE instance_id = @InstanceID;";
            List<bool> statusList = _data.LoadData<bool, dynamic>(sql, new { InstanceID = courseInstanceID }, _config.GetConnectionString("Default"));
            return statusList;
        }

        public static bool GetCourseInstanceReadOnlyStatus(IConfiguration _config, IDataAccess _data, int courseInstanceID)
        {
            string sql = "SELECT is_read_only FROM course_instance WHERE instance_id = @InstanceID;";
            List<bool> statusList = _data.LoadData<bool, dynamic>(sql, new { InstanceID = courseInstanceID }, _config.GetConnectionString("Default"));
            if (statusList is null || statusList.Count == 0)
            {
                return false;
            }
            return statusList[0];
        }
    }
}
