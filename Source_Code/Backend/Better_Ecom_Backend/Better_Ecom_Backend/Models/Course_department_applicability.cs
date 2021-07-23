using System;
using System.Text.Json;

namespace Better_Ecom_Backend.Models
{
    public class Course_department_applicability
    {
        public string Course_code { get; set; }
        public string Deparmtent_code { get; set; }

        public Course_department_applicability(string courseCode, string departmentCode)
        {
            Course_code = courseCode;
            Deparmtent_code = departmentCode;
        }

        public Course_department_applicability() : this(null, null) { }

        public Course_department_applicability(JsonElement jsonData)
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
