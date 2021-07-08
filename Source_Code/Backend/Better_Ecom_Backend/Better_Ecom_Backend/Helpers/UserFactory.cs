using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Better_Ecom_Backend.Models;

namespace Better_Ecom_Backend.Helpers
{
    public static class UserFactory
    {
        public static System_user getUser(JsonElement data, string type)
        {
            if (type == "student")
            {
                return new Student(data);
            }
            else if (type == "instructor")
            {
                return new Instructor(data);
            }
            else if (type == "admin")
            {
                return new Admin_user(data);
            }
            else
            {
                return null;
            }
        }
    }
}
