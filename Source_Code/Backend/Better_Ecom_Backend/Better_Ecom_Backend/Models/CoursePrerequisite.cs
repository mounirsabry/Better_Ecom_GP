using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Better_Ecom_Backend.Models
{
    public class CoursePrerequisite
    {
        public string Course_code { get; set; }
        public string Prerequisite_course_code { get; set; }

        public CoursePrerequisite(string courseCode, string prerequisiteCourseCode)
        {
            Course_code = courseCode;
            Prerequisite_course_code = prerequisiteCourseCode;
        }

        public CoursePrerequisite() : this(null, null) { }

        public CoursePrerequisite(JsonElement jsonData)
        {
            JsonElement temp;
            if (jsonData.TryGetProperty(nameof(Course_code), out temp))
                Course_code = temp.GetString();
            else
                Course_code = null;

            if (jsonData.TryGetProperty(nameof(Prerequisite_course_code), out temp))
                Prerequisite_course_code = temp.GetString();
            else
                Prerequisite_course_code = null;
        }

        public void Print()
        {
            Console.WriteLine("Course Prequisite Info.");
            Console.WriteLine($"Course Code{ Course_code }.");
            Console.WriteLine($"Prerequisite Course Code :{ Prerequisite_course_code }");
        }
    }
}
