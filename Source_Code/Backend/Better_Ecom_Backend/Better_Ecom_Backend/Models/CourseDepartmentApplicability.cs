using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Better_Ecom_Backend.Models
{
    public class CourseDepartmentApplicability
    {
        public string Course_code { get; set; }
        public string Deparmtent_code { get; set; }

        public CourseDepartmentApplicability(string courseCode, string departmentCode)
        {
            Course_code = courseCode;
            Deparmtent_code = departmentCode;
        }

        public CourseDepartmentApplicability() : this(null, null) { }

        public CourseDepartmentApplicability(JsonElement jsonData)
        {
            JsonElement temp;
            if (jsonData.TryGetProperty(nameof(Course_code), out temp))
                Course_code = temp.GetString();
            else
                Course_code = null;

            if (jsonData.TryGetProperty(nameof(Deparmtent_code), out temp))
                Deparmtent_code = temp.GetString();
            else
                Deparmtent_code = null;
        }

        public void Print()
        {
            Console.WriteLine("Course Department Applicability Info.");
            Console.WriteLine($"Course Code :{ Course_code }.");
            Console.WriteLine($"Department Code :{ Deparmtent_code }");
        }
    }
}
