using System;
using System.Text.Json;

namespace Better_Ecom_Backend.Models
{
    public class Department
    {
        public string Department_code { get; set; }
        public string Department_name { get; set; }

        public Department(string departmentCode, string departmentName)
        {
            Department_code = departmentCode;
            Department_name = departmentName;
        }

        public Department() : this(null, null)
        { }

        public Department(JsonElement data)
        {
            JsonElement temp;
            if (data.TryGetProperty(nameof(Department_code), out temp))
                Department_code = temp.GetString();
            else
                Department_code = null;

            if (data.TryGetProperty(nameof(Department_name), out temp))
                Department_name = temp.GetString();
            else
                Department_name = null;
        }

        public void Print()
        {
            Console.WriteLine($"Department :{ Department_code }.");
        }
    }
}
